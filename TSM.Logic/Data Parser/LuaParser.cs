using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using TSM.Logic.Data_Parser.Models;

namespace TSM.Logic.Data_Parser
{
    internal class LuaParser
    {
        public static async Task<LuaModel> ParseLua(FileInfo fileInfo)
        {
            const string regex = @"[\r\n\t]+";
            if (fileInfo is null) throw new ArgumentNullException(nameof(fileInfo));
            if (!fileInfo.Exists) throw new FileNotFoundException(fileInfo.FullName);

            LuaModel topLevel = new();

            using FileStream fs = new(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int read;

            LuaModel currentLuaModel = topLevel;
            bool openQuote = false;
            StringBuilder tempValue = new();
            while ((read = await fs.ReadAsync(buffer.AsMemory(0, buffer.Length))) > 0)
            {
                string data = Encoding.UTF8.GetString(buffer, 0, read);
                data = Regex.Replace(data, regex, string.Empty);

                while (data.Length > 0)
                {
                    int nextEquals = data.IndexOf('=');
                    int nextComma = openQuote ? -1 : data.IndexOf(',');
                    int nextQuote = data.IndexOf('"');
                    int nextCloseBrace = data.IndexOf('}');
                    int nextOpenBrace = data.IndexOf('{');

                    int nextOperatorIndex = new int[] { nextEquals, nextComma, nextQuote, nextCloseBrace, nextOpenBrace, data.Length - 1 }.Where(i => i >= 0).Min();
                    char nextOperator = data[nextOperatorIndex];

                    switch (nextOperator)
                    {
                        case '=':
                            if (currentLuaModel.Parent == null)
                            {
                                LuaModel lm = new();
                                currentLuaModel.AddChild(lm);
                                currentLuaModel = lm;
                            }
                            else if (currentLuaModel.Parent == topLevel)
                            {
                                LuaModel lm = new();
                                topLevel.AddChild(lm);
                                currentLuaModel = lm;
                            }

                            currentLuaModel.Key = (tempValue.Length == 0 ? data[0..nextOperatorIndex] : tempValue.ToString()).Trim(new char[] { ' ', '[', ']', '"' });

                            tempValue.Clear();
                            break;
                        case ',':
                            if (!openQuote)
                            {
                                string value = (tempValue.Length == 0 ? data[0..nextOperatorIndex] : tempValue.ToString()).Trim(new char[] { '\r', '\n', ' ', '{', '}' });
                                tempValue.Clear();
                                if (!string.IsNullOrEmpty(value))
                                {
                                    currentLuaModel.Value = value;
                                }

                                LuaModel lm = new();
                                currentLuaModel.Parent.AddChild(lm);
                                currentLuaModel = lm;
                            }
                            break;
                        case '"':
                            if (openQuote)
                            {
                                tempValue.Append(data[0..nextOperatorIndex]);
                            }

                            openQuote = !openQuote;
                            break;
                        case '}':
                            currentLuaModel = currentLuaModel.Parent;
                            break;
                        case '{':
                            if (currentLuaModel.Parent != null)
                            {
                                LuaModel lm = new();
                                currentLuaModel.AddChild(lm);
                                currentLuaModel = lm;
                            }
                            break;
                        default:
                            tempValue.Append(data[0..nextOperatorIndex]);
                            break;
                    }

                    if (nextOperatorIndex + 1 <= data.Length)
                    {
                        data = data[(nextOperatorIndex + 1)..];
                    }
                    else
                    {
                        data = String.Empty;
                    }
                }
            }

            topLevel.ClearEmptyChildren();

            return topLevel;
        }
    }
}