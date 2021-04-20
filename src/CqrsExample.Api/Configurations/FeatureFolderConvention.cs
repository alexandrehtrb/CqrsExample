using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace CqrsExample.Api.Configurations
{
    public class FeatureFolderConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel model) =>
            model.Properties.Add("feature", GetFeatureName(model));
        
        private string GetFeatureName(ControllerModel model)
        {
            string[] tokens = (model.ControllerType.FullName ?? "").Split('.');
            if (!tokens.Any(t => t == "Features")) return "";
            return (tokens
                    .SkipWhile(t => !t.Equals("features", StringComparison.CurrentCultureIgnoreCase))
                    .Skip(1)
                    .Take(1)
                    .FirstOrDefault() ?? "");
        }
    }
}