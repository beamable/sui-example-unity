using System.IO;
using System.Threading.Tasks;
using Beamable.Microservices.SuiFederation.Features.Contracts.Models;
using HandlebarsDotNet;

namespace Beamable.Microservices.SuiFederation.Features.Contracts
{
    public class ContractTemplateService : IService
    {
        private const string ItemTemplateFile = "move/templates/item.move";
        private const string CurrencyTemplateFile = "move/templates/currency.move";
        private bool _initialized;

        public ContractTemplateService()
        {
            if (!_initialized)
                Initialize();
        }

        private void Initialize()
        {
            Handlebars.RegisterHelper("toUpperCase", (writer, context, parameters) => {
                var text = parameters[0].ToString().ToUpper();
                writer.Write(text);
            });

            Handlebars.RegisterHelper("toLowerCase", (writer, context, parameters) => {
                var text = parameters[0].ToString().ToLower();
                writer.Write(text);
            });

            Handlebars.RegisterHelper("toStructName", (writer, context, parameters) => {
                var text = parameters[0].ToString().Replace("_","").Replace("-", "").Replace(" ", "");
                var result = char.ToUpper(text[0]) + text.Substring(1);
                writer.Write(result);
            });

            _initialized = true;
        }

        public async Task GenerateItemContract(IModuleData data)
        {
            var dataInstance = data as ItemModuleData;
            var itemTemplate = await File.ReadAllTextAsync(ItemTemplateFile);
            var template = Handlebars.Compile(itemTemplate);
            var itemResult = template(dataInstance);
            await File.WriteAllTextAsync($"move/sources/{dataInstance.module_name}.move", itemResult);
        }

        public async Task GenerateCurrencyContract(IModuleData data)
        {
            var dataInstance = data as CurrencyModuleData;
            var currencyTemplate = await File.ReadAllTextAsync(CurrencyTemplateFile);
            var template = Handlebars.Compile(currencyTemplate);
            var currencyResult = template(dataInstance);
            await File.WriteAllTextAsync($"move/sources/{dataInstance.module_name}.move", currencyResult);
        }
    }
}