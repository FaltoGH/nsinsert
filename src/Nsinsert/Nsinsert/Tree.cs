namespace Nsinsert
{
    internal class Tree
    {
        public readonly bool IsFile;
        public readonly string Location;

        private readonly Lazy<List<Tree>> _children;
        public List<Tree> Children => _children.Value;

        private Tree? _parent;
        public Tree? Parent => _parent;

        public Tree(bool isFile, string location)
        {
            IsFile = isFile;
            Location = location;

            _children = new Lazy<List<Tree>>(BuildChildren, false);
        }


        private List<Tree> BuildChildren()
        {
            List<Tree> children = new List<Tree>();

            if (!IsFile)
            {
                string[] fileSystemEntries = Directory.GetFileSystemEntries(Location);

                foreach (var f in fileSystemEntries)
                {
                    bool isFile = File.Exists(f);
                    Tree child = new Tree(isFile, f);
                    child._parent = this;
                    children.Add(child);
                }
            }

            children.Sort((a, b) =>
            {
                int compareResult = b.IsFile.CompareTo(a.IsFile);
                if (compareResult == 0)
                {
                    compareResult = a.Location.CompareTo(b.Location);
                }
                return compareResult;
            });

            return children;
        }


        public Tree GetRoot()
        {
            Tree t = this;

            while(t.Parent != null)
            {
                t = t.Parent;
            }

            return t;
        }


        public IEnumerable<string> GetFilesRecursive()
        {
            if (IsFile) { throw new InvalidOperationException(); }

            Stack<Tree> stack = new Stack<Tree>();
            stack.Push(this);

            while (stack.TryPop(out Tree? result))
            {
                if (result.IsFile)
                {
                    throw new InvalidOperationException();
                }

                foreach (var child in result.Children)
                {
                    if (child.IsFile)
                    {
                        yield return child.Location;
                    }
                    else
                    {
                        stack.Push(child);
                    }
                }
            }

        }
    }
}
