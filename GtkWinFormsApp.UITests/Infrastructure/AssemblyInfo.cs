// GTK is single-threaded: every widget touched by these tests must be created and driven on one thread.
// xUnit runs test classes in parallel by default, which would call into GTK from several threads at once
// and crash the host. Serialize the whole assembly.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
