using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Json;
namespace System.AspnetCore
{
    public class SearchConditionBinder : IModelBinder
    {
        private string Magic_Prefix = "$";
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(SearchCondition) ||
                bindingContext.HttpContext.Request.Method.ToUpper()!="GET")
            {
                return Task.CompletedTask;
            }
            string modelName = bindingContext.ModelName;

            var sampleQuery = bindingContext.HttpContext.Request.Query
                                .Where(p => p.Key.StartsWith(Magic_Prefix))
                                .Select(p => Tuple.Create(p.Key.Substring(Magic_Prefix.Length), p.Value.Last()));
                              
            var scItems = new List<SearchCondition>();
            foreach (var query in sampleQuery)
            {
                scItems.Add(new SearchCondition(query.Item1, SearchType.Equals, query.Item2));
            }

            if (string.IsNullOrEmpty(modelName))
            {
                var queryObjects = bindingContext.ValueProvider.GetValue(modelName).Values.ToArray();
                foreach (var base64Item in queryObjects)
                {
                    if (string.IsNullOrEmpty(base64Item)) continue;
                    
                    var bytes = Convert.FromBase64String(base64Item);
                    var jsonString = Encoding.UTF8.GetString(bytes);
                    scItems.Add(jsonString.AsJsonObject<SearchCondition>());
                }
            }


            if (scItems.Count > 0)
            {
                bindingContext.Result= ModelBindingResult.Success( new SearchCondition(scItems, OpType.AndItems));
            }

            return Task.CompletedTask;
        }
    }
}
