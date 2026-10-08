using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Nsinsert
{
    public class Inserter
    {
        private readonly IReadOnlyList<string> _lines;

        public Inserter(IReadOnlyList<string> lines)
        {
            _lines = lines;
        }


        public List<string> Insert()
        {
            var regexDir = new Regex("nsinsert dir (.+)");
            string dir = string.Empty;
            Tree tree = new Tree(false, string.Empty);
            bool dirSet = false;
            var ret = new List<string>();

            foreach (var line in _lines)
            {
                string lineTrimmed = line.Trim();
                Match match = regexDir.Match(lineTrimmed);

                if (match.Success)
                {
                    if (dirSet)
                    {
                        throw new InvalidOperationException("dir already set");
                    }

                    dir = match.Groups[1].Value;

                    if (!Directory.Exists(dir))
                    {
                        throw new DirectoryNotFoundException(dir);
                    }

                    tree = new Tree(false, dir);
                    dirSet = true;
                }
                else if(lineTrimmed == "nsinsert file")
                {
                    if (!dirSet)
                    {
                        throw new InvalidOperationException("cannot insert file without dir");
                    }

                    ret.AddRange(GetLinesFile(tree));
                }
                else if(lineTrimmed == "nsinsert delete")
                {
                    if (!dirSet)
                    {
                        throw new InvalidOperationException("cannot insert delete without dir");
                    }

                    ret.AddRange(GetLinesDelete(tree));
                }
                else
                {
                    ret.Add(line);
                }
            }

            return ret;
        }


        private static List<string> GetLinesFile(Tree tree)
        {
            var ret = new List<string>();
            VisitForFile(tree, ret);
            ret.Insert(0, $";nsinsert: begin file; Count={ret.Count}");
            ret.Add(";nsinsert: end file");
            return ret;
        }


        private static void VisitForFile(Tree tree, List<string> ret)
        {
            string instdir = tree.Location.Substring(tree.GetRoot().Location.Length);
            ret.Add($"  SetOutPath \"$INSTDIR{instdir}\"");

            foreach (var child in tree.Children)
            {
                if (child.IsFile)
                {
                    ret.Add($"  File \"{child.Location}\"");
                }
                else
                {
                    VisitForFile(child, ret);
                }
            }
        }

        private static List<string> GetLinesDelete(Tree tree)
        {
            var ret = new List<string>();
            string rootLocation = tree.GetRoot().Location;

            foreach (var file in tree.GetFilesRecursive())
            {
                string instfile = file.Substring(rootLocation.Length);
                ret.Add($"  Delete \"$INSTDIR{instfile}\"");
            }

            VisitForRMDir(tree, ret);
            ret.Insert(0, $";nsinsert: begin delete; Count={ret.Count}");
            ret.Add(";nsinsert: end delete");
            return ret;
        }

        private static void VisitForRMDir(Tree tree, List<string> ret)
        {
            foreach(var child in tree.Children)
            {
                if (!child.IsFile)
                {
                    VisitForRMDir(child, ret);
                }
            }

            string instdir = tree.Location.Substring(tree.GetRoot().Location.Length);
            ret.Add($"  RMDir \"$INSTDIR{instdir}\"");
        }


    }
}
