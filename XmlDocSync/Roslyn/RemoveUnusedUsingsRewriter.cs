using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XmlDocSync.Roslyn;

public class RemoveUnusedUsingsRewriter : CSharpSyntaxRewriter
{
    private readonly SemanticModel _semanticModel;
    private readonly HashSet<string> _preservedUsings;
    private readonly CompilationUnitSyntax _root;

    public RemoveUnusedUsingsRewriter(
        SemanticModel semanticModel,
        HashSet<string> preservedUsings,
        CompilationUnitSyntax root)
    {
        _semanticModel = semanticModel;
        _preservedUsings = preservedUsings;
        _root = root;
    }

    public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
    {
        // Preserve leading trivia from the first using directive
        var leadingTrivia = SyntaxTriviaList.Empty;
        if (node.Usings.Count > 0)
        {
            leadingTrivia = node.Usings[0].GetLeadingTrivia();
        }

        // Keep usings that are used OR that are special (static / alias / preserved)
        var newUsings = node.Usings
            .Where(u => IsUsingUsed(u))
            .ToList();

        // If we have usings left, attach the preserved trivia to the first one
        if (newUsings.Count > 0 && leadingTrivia.Count > 0)
        {
            newUsings[0] = newUsings[0].WithLeadingTrivia(leadingTrivia);
        }

        var result = node.WithUsings(SyntaxFactory.List(newUsings));

        // If all usings were removed, attach the trivia to the next element
        if (newUsings.Count == 0 && leadingTrivia.Count > 0)
        {
            // Try to attach to extern aliases, attributes, or members
            if (result.Externs.Count > 0)
            {
                var firstExtern = result.Externs[0];
                result = result.WithExterns(
                    result.Externs.Replace(firstExtern, 
                        firstExtern.WithLeadingTrivia(leadingTrivia.AddRange(firstExtern.GetLeadingTrivia()))));
            }
            else if (result.AttributeLists.Count > 0)
            {
                var firstAttr = result.AttributeLists[0];
                result = result.WithAttributeLists(
                    result.AttributeLists.Replace(firstAttr,
                        firstAttr.WithLeadingTrivia(leadingTrivia.AddRange(firstAttr.GetLeadingTrivia()))));
            }
            else if (result.Members.Count > 0)
            {
                var firstMember = result.Members[0];
                result = result.WithMembers(
                    result.Members.Replace(firstMember,
                        firstMember.WithLeadingTrivia(leadingTrivia.AddRange(firstMember.GetLeadingTrivia()))));
            }
        }

        return result;
    }

    private bool IsUsingUsed(UsingDirectiveSyntax usingDirective)
    {
        // Always preserve:
        // 1. static usings
        // 2. alias usings
        // 3. usings explicitly preserved in JSON
        if (usingDirective.StaticKeyword.Kind() != Microsoft.CodeAnalysis.CSharp.SyntaxKind.None)
            return true;

        if (usingDirective.Alias != null)
            return true;

        var namespaceName = usingDirective.Name.ToString();

        if (_preservedUsings.Contains(namespaceName))
            return true;

        // If there are any preprocessor directives in the file, preserve the using
        // because code might be hidden in #if blocks that we can't see
        if (_root.DescendantTrivia().Any(t => t.IsDirective))
            return true;

        // Track if we encounter any unresolved identifiers that could potentially be from this namespace
        bool hasUnresolvedIdentifiers = false;

        // Check all descendant nodes that could reference symbols
        var allNodes = _root.DescendantNodes();

        foreach (var node in allNodes)
        {
            ISymbol? symbol = null;

            // Get symbol based on node type
            switch (node)
            {
                case IdentifierNameSyntax identifier:
                    symbol = _semanticModel.GetSymbolInfo(identifier).Symbol;
                    if (symbol == null)
                    {
                        // Track unresolved identifiers
                        hasUnresolvedIdentifiers = true;
                        // Fallback: check if identifier text suggests it might be from this namespace
                        if (IsLikelyFromNamespace(identifier.Identifier.Text, namespaceName))
                            return true;
                    }
                    break;
                case MemberAccessExpressionSyntax memberAccess:
                    symbol = _semanticModel.GetSymbolInfo(memberAccess).Symbol;
                    // Fallback for known types in common namespaces
                    if (symbol == null)
                    {
                        hasUnresolvedIdentifiers = true;
                        var name = memberAccess.Expression.ToString();
                        if (IsLikelyFromNamespace(name, namespaceName))
                            return true;
                    }
                    break;
                case AttributeSyntax attribute:
                    symbol = _semanticModel.GetSymbolInfo(attribute).Symbol;
                    // Fallback: check attribute name
                    if (symbol == null)
                    {
                        hasUnresolvedIdentifiers = true;
                        var attrName = GetSimpleAttributeName(attribute.Name);
                        if (IsLikelyAttributeFromNamespace(attrName, namespaceName))
                            return true;
                    }
                    break;
                case QualifiedNameSyntax qualifiedName:
                    symbol = _semanticModel.GetSymbolInfo(qualifiedName).Symbol;
                    if (symbol == null)
                        hasUnresolvedIdentifiers = true;
                    break;
                case GenericNameSyntax genericName:
                    symbol = _semanticModel.GetSymbolInfo(genericName).Symbol;
                    if (symbol == null)
                    {
                        hasUnresolvedIdentifiers = true;
                        // Fallback for generic types
                        if (IsLikelyFromNamespace(genericName.Identifier.Text, namespaceName))
                            return true;
                    }
                    break;
                case ObjectCreationExpressionSyntax objectCreation:
                    symbol = _semanticModel.GetSymbolInfo(objectCreation).Symbol;
                    if (symbol == null)
                        hasUnresolvedIdentifiers = true;
                    break;
                case PropertyDeclarationSyntax property:
                    // Check the property type
                    var typeInfo = _semanticModel.GetTypeInfo(property.Type);
                    symbol = typeInfo.Type;
                    if (symbol == null)
                        hasUnresolvedIdentifiers = true;
                    break;
                case ParameterSyntax parameter:
                    // Check the parameter type
                    if (parameter.Type != null)
                    {
                        var paramTypeInfo = _semanticModel.GetTypeInfo(parameter.Type);
                        symbol = paramTypeInfo.Type;
                        if (symbol == null)
                            hasUnresolvedIdentifiers = true;
                    }
                    break;
                case FieldDeclarationSyntax field:
                    // Check the field type
                    var fieldTypeInfo = _semanticModel.GetTypeInfo(field.Declaration.Type);
                    symbol = fieldTypeInfo.Type;
                    if (symbol == null)
                        hasUnresolvedIdentifiers = true;
                    break;
                case EventDeclarationSyntax eventDecl:
                    // Check the event type
                    var eventTypeInfo = _semanticModel.GetTypeInfo(eventDecl.Type);
                    symbol = eventTypeInfo.Type;
                    if (symbol == null)
                        hasUnresolvedIdentifiers = true;
                    break;
                case EventFieldDeclarationSyntax eventField:
                    // Check the event field type
                    var eventFieldTypeInfo = _semanticModel.GetTypeInfo(eventField.Declaration.Type);
                    symbol = eventFieldTypeInfo.Type;
                    if (symbol == null)
                        hasUnresolvedIdentifiers = true;
                    break;
                case BaseTypeSyntax baseType:
                    // Check base class and interface implementations
                    var baseTypeInfo = _semanticModel.GetTypeInfo(baseType.Type);
                    symbol = baseTypeInfo.Type;
                    if (symbol == null)
                    {
                        hasUnresolvedIdentifiers = true;
                        // Fallback: check if the base type name suggests it's from this namespace
                        var baseTypeName = GetSimpleTypeName(baseType.Type);
                        if (IsLikelyFromNamespace(baseTypeName, namespaceName))
                            return true;
                    }
                    break;
            }

            if (symbol != null)
            {
                var containingNamespace = symbol.ContainingNamespace?.ToDisplayString();

                // Check if symbol is from this namespace or a child namespace
                if (containingNamespace != null &&
                    (containingNamespace == namespaceName ||
                     containingNamespace.StartsWith(namespaceName + ".")))
                {
                    return true;
                }
            }
        }

        // Conservative fallback: if we have unresolved identifiers and this is not a well-known
        // .NET namespace, keep the using to be safe (external libraries like Gtk, custom assemblies)
        if (hasUnresolvedIdentifiers && !IsWellKnownDotNetNamespace(namespaceName))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Extracts the simple name from an attribute name syntax, handling qualified names.
    /// </summary>
    private string GetSimpleAttributeName(NameSyntax nameSyntax)
    {
        return nameSyntax switch
        {
            QualifiedNameSyntax qualifiedName => qualifiedName.Right.Identifier.Text,
            IdentifierNameSyntax identifierName => identifierName.Identifier.Text,
            GenericNameSyntax genericName => genericName.Identifier.Text,
            _ => nameSyntax.ToString()
        };
    }

    /// <summary>
    /// Extracts the simple name from a type syntax, handling qualified names and generic types.
    /// </summary>
    private string GetSimpleTypeName(TypeSyntax typeSyntax)
    {
        return typeSyntax switch
        {
            QualifiedNameSyntax qualifiedName => GetSimpleTypeName(qualifiedName.Right),
            IdentifierNameSyntax identifierName => identifierName.Identifier.Text,
            GenericNameSyntax genericName => genericName.Identifier.Text,
            SimpleNameSyntax simpleName => simpleName.Identifier.Text,
            _ => typeSyntax.ToString()
        };
    }

    private string GetSimpleTypeName(SimpleNameSyntax nameSyntax)
    {
        return nameSyntax switch
        {
            IdentifierNameSyntax identifierName => identifierName.Identifier.Text,
            GenericNameSyntax genericName => genericName.Identifier.Text,
            _ => nameSyntax.ToString()
        };
    }

    /// <summary>
    /// Fallback heuristic when semantic model can't resolve symbols due to missing references.
    /// Checks if a type name is likely from a specific namespace based on common patterns.
    /// </summary>
    private bool IsLikelyFromNamespace(string typeName, string namespaceName)
    {
        // Common types in System.Collections.Generic
        if (namespaceName == "System.Collections.Generic")
        {
            return typeName is "List" or "Dictionary" or "HashSet" or "Queue" or "Stack" 
                or "LinkedList" or "IEnumerable" or "ICollection" or "IList" or "IDictionary"
                or "ISet" or "IComparer" or "IEqualityComparer" or "KeyValuePair";
        }

        if (namespaceName == "System.Collections")
        {
            return typeName is "ReadOnlyCollectionBase";
        }

        if (namespaceName == "System.Text.RegularExpressions")
        {
            return typeName is "Regex";
        }

        if (namespaceName == "System.Collections.Specialized")
        {
            return typeName is "StringCollection" or "BitVector32";
        }

        // Common types in System.Runtime.CompilerServices
        if (namespaceName == "System.Runtime.CompilerServices")
        {
            return typeName is "BitOperations" or "Unsafe" or "RuntimeHelpers";
        }

        // Common types in System.Linq
        if (namespaceName == "System.Linq")
        {
            return typeName is "Enumerable" or "Queryable";
        }

        // Common types in System.Drawing
        if (namespaceName == "System.Drawing")
        {
            return typeName is "Size" or "Point" or "Color" or "Rectangle" or "RectangleF"
                or "SizeF" or "PointF" or "Bitmap" or "Image" or "Graphics" or "Brush"
                or "Pen" or "Font" or "Icon" or "SolidBrush" or "TextureBrush";
        }

        // Common types in System.ComponentModel
        if (namespaceName == "System.ComponentModel")
        {
            return typeName is "Component" or "IComponent" or "Container" or "IContainer"
                or "INotifyPropertyChanged" or "INotifyPropertyChanging" or "PropertyChangedEventHandler"
                or "PropertyChangingEventHandler" or "TypeConverter" or "MarshalByValueComponent"
                or "CancelEventArgs" or "CancelEventHandler" or "EventDescriptor" or "PropertyDescriptor"
                or "CollectionChangeEventArgs" or "ComponentConverter" or "ExpandableObjectConverter" 
                or "HandledEventArgs" or "ListChangedType" or "MaskedTextResultHint" or "PropertyTabScope" 
                or "CategoryAttribute" or "BrowsableAttribute";
        }

        if (namespaceName == "System.ComponentModel.Design")
        {
            return typeName is "ITypeResolutionService";
        }

        if (namespaceName == "System.ComponentModel.Design.Serialization")
        {
            return typeName is "InstanceDescriptor";
        }

        if (namespaceName == "System.Windows.Forms.Design")
        {
            return typeName is "PropertyTab";
        }

        if (namespaceName == "System.Windows.Forms.Layout")
        {
            return typeName is "IArrangedElement";
        }

        return false;
    }

    /// <summary>
    /// Checks if an attribute name suggests it's from a specific namespace.
    /// </summary>
    private bool IsLikelyAttributeFromNamespace(string attributeName, string namespaceName)
    {
        // Remove "Attribute" suffix if present
        var name = attributeName.EndsWith("Attribute") 
            ? attributeName.Substring(0, attributeName.Length - 9) 
            : attributeName;

        // System.Runtime.CompilerServices attributes
        if (namespaceName == "System.Runtime.CompilerServices")
        {
            return name is "MethodImpl" or "CompilerGenerated" or "CallerMemberName" 
                or "CallerFilePath" or "CallerLineNumber" or "InternalsVisibleTo"
                or "AggressiveInlining" or "Extension" or "AsyncStateMachine" or "IteratorStateMachine";
        }

        // System.Diagnostics attributes
        if (namespaceName == "System.Diagnostics")
        {
            return name is "Conditional" or "DebuggerDisplay" or "DebuggerHidden" 
                or "DebuggerStepThrough" or "DebuggerBrowsable";
        }

        // System.ComponentModel attributes
        if (namespaceName == "System.ComponentModel")
        {
            return name is "Browsable" or "Category" or "DefaultValue" or "Description"
                or "DisplayName" or "EditorBrowsable" or "TypeConverter" or "Bindable"
                or "DesignerCategory" or "DefaultEvent" or "DesignTimeVisible" or "ToolboxItemFilter" 
                or "ToolboxItem";
        }

        return false;
    }

    /// <summary>
    /// Checks if a namespace is a well-known .NET Framework/Core namespace.
    /// These can be safely removed if unused, as opposed to external library namespaces.
    /// </summary>
    private bool IsWellKnownDotNetNamespace(string namespaceName)
    {
        // Check if it starts with "System" or "Microsoft"
        return namespaceName.StartsWith("System.") || 
               namespaceName.StartsWith("System") ||
               namespaceName.StartsWith("Microsoft.");
    }
}