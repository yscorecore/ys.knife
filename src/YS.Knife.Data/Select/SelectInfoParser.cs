using System;
using System.Collections.Generic;
using System.Text;

namespace YS.Knife.Data
{
    internal class SelectInfoParser
    {
        public SelectInfo ParseSelectInfo(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text);
            SelectInfo selectInfo= ParseSelectInfo(context);
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return selectInfo;
        }
        private SelectInfo ParseSelectInfo(ParseContext context)
        {
            SelectInfo selectInfo = new SelectInfo
            {
                Items = new List<SelectItem>()
            };
            while (context.SkipWhiteSpace())
            {
                var (found, name) = context.TryParseName();
                if (found)
                {
                    selectInfo.Items.Add(ParseSelectItem(name, context));
                    if (context.SkipWhiteSpace() && context.Current() == ',')
                    {
                        context.Index++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return selectInfo;
        }
        private SelectItem ParseSelectItem(string name, ParseContext context)
        {
            SelectItem item = new SelectItem { Name = name };
            if (context.SkipWhiteSpace() &&context.Current() == '{')
            {
                // parse collection infos
                ParseCollectionInfos(item, context);
            }
            if (context.SkipWhiteSpace() && context.Current() == '(')
            {
                // parse sub items
                context.Index++;
                item.SubItems = ParseSelectInfo(context).Items;
                if (context.SkipWhiteSpace() == false || context.Current() != ')')
                {
                    throw ParseErrors.MissCloseBracket(context);
                }
                else
                {
                    context.Index++;
                }
            }
            return item;
        }
        private void ParseCollectionInfos(SelectItem selectItem,ParseContext context)
        {
            context.Index++;
            //skip start {
            if (context.SkipWhiteSpace())
            {
                if (context.Current() == '(')
                {
                    // parse filter info
                }
                else if (ParseContext.IsValidNameFirstChar(context.Current()))
                {
                    //maybe filter,or order info
                    var (_, name) = context.TryParseName();

                }
                else if (char.IsDigit(context.Current()))
                {
                    // limit info, or filter info
                }
                else
                {
                    throw ParseErrors.InvalidText(context);
                }

                // {a=b,c,true,,1,2}

            }
            else
            {
                throw ParseErrors.InvalidText(context);
            }
        }
        
       
    }
}
