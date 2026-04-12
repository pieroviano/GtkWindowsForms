# Copilot Instructions

## Code Style
- Use specific formatting rules
- Follow naming conventions
- Prefer implementations that mimic System.Drawing semantics for gradient brushes (PathGradientBrush) – use Matrix operations and clone semantics similar to System.Drawing.

## Error Handling
- When handling `Gdk.Pixbuf.SaveToBuffer` failures, catch `GLib.GException` and fallback to null `PixbufData`, logging via `System.Diagnostics.GtkLogger.Debug`.

## Documentation
- Ensure that XML comments are added for all fields, including the 'self' field.
- Prefer fixes that handle `FieldDeclaration` by checking `VariableDeclarator` symbols when updating documentation.