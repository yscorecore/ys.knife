using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YS.Knife.Data.Translaters
{
    public class FieldPath
    {
        public string Field { get; set; }

        public string FuncName { get; set; }

        public bool IsFunction { get => !string.IsNullOrEmpty(FuncName); }
        public List<FieldPath> SubPaths { get; set; }

        public static List<FieldPath> ParsePaths(string fieldNames)
        {
            return new FieldSplitter().Split(fieldNames);
        }

        internal class FieldSplitter
        {
            public List<FieldPath> Split(string filedName)
            {
                if (string.IsNullOrWhiteSpace(filedName))
                {
                    throw new FieldExpressionException($"Field name to split should not be blank.");
                }

                List<FieldPath> result = new List<FieldPath>();
                StringBuilder tempPaths = new StringBuilder();
                bool inFunction = false;
                bool lastIsSpace = false;
                for (int i = 0; i < filedName.Length; i++)
                {
                    char ch = filedName[i];
                    switch (ch)
                    {
                        case '.':
                            if (tempPaths.Length > 0)
                            {
                                if (inFunction && result.Count > 0)
                                {
                                    result.Last().SubPaths.Add(new FieldPath() {Field = tempPaths.ToString()});
                                }
                                else
                                {
                                    result.Add(new FieldPath() {Field = tempPaths.ToString()});
                                }

                                tempPaths.Clear();
                            }
                            break;
                        case '(':
                            if (inFunction == false && tempPaths.Length > 0)
                            {
                                result.Add(new FieldPath()
                                {
                                    FuncName = tempPaths.ToString(), SubPaths = new List<FieldPath>()
                                });
                                tempPaths.Clear();
                                inFunction = true;
                            }
                            else
                            {
                                ThrowInvalidPath(i);
                            }
                            break;
                        case ')':
                            if (inFunction == true && result.Count > 0)
                            {
                                if (tempPaths.Length > 0)
                                {
                                    result.Last().SubPaths.Add(new FieldPath() {Field = tempPaths.ToString()});
                                }

                                tempPaths.Clear();
                                inFunction = false;
                            }
                            else
                            {
                                ThrowInvalidPath(i);
                            }
                            break;
                        case ' ':
                        case '\t':
                            lastIsSpace = true;
                            break;

                        default:
                            if (tempPaths.Length == 0)
                            {
                                if (!char.IsWhiteSpace(ch))
                                {
                                    tempPaths.Append(ch);
                                    lastIsSpace = false;
                                }
                            }
                            else
                            {
                                if (char.IsWhiteSpace(ch))
                                {
                                    lastIsSpace = true;
                                }
                                else
                                {
                                    if (lastIsSpace)
                                    {
                                        ThrowInvalidPath(i - 1);
                                    }
                                    else
                                    {
                                        tempPaths.Append(ch);
                                    }
                                }
                            }
                            break;
                    }
                }

                if (inFunction)
                {
                    ThrowInvalidPath(filedName.Length + 1);
                }

                if (tempPaths.Length > 0)
                {
                    result.Add(new FieldPath {Field = tempPaths.ToString()});
                }

                return result;
            }

            private void ThrowInvalidPath(int index)
            {
                var exception = new FieldExpressionException($"Invalid field path at index: {index}.");
                exception.Data[nameof(index)] = index;
                throw exception;
            }
            
        }
    }
}
