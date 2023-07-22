using Microsoft.AspNetCore.Mvc.Rendering;
using OregonNexus.Broker.Domain;
using OregonNexus.Broker.Domain.Specifications;
using OregonNexus.Broker.SharedKernel;
using IConfiguration = OregonNexus.Broker.Connector.Configuration.IConfiguration;
using System.ComponentModel.DataAnnotations;

namespace OregonNexus.Broker.Web.Helpers;

public class ModelFormBuilderHelper
{
    public static string HtmlForModel(IConfiguration model)
    {
        var formHTML = ""; // start form html output

        // Get type of incoming model
        var modelType = model.GetType();
        // Get the properties of the model
        var modelTypeProps = modelType.GetProperties();
        // For each, determine input html needed
        foreach(var modelTypeProp in modelTypeProps)
        {
            // include existing value if set in model

            // Write HTML
            var modelTypePropAttrsDataType = (DataTypeAttribute)modelTypeProp.GetCustomAttributes(false).Where(x => x.GetType() == typeof(DataTypeAttribute)).FirstOrDefault()!;

            if (modelTypePropAttrsDataType.DataType == DataType.Url)
            {
                formHTML += $"""
                <div class="sm:col-span-4 my-4">
              <label for="{modelTypeProp.Name}" class="block text-sm font-medium leading-6 text-gray-900">{modelTypeProp.Name}</label>
              <div class="mt-2">
                <div class="flex rounded-md shadow-sm ring-1 ring-inset ring-gray-300 focus-within:ring-2 focus-within:ring-inset focus-within:ring-indigo-600 sm:max-w-md">
                  <input type="text" name="{modelTypeProp.Name}" id="{modelTypeProp.Name}" value="{modelTypeProp.GetValue(model)}" class="block w-full rounded-md border-0 p-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6">
                </div>
              </div>
              </div>
              """;
            }

            if (modelTypePropAttrsDataType.DataType == DataType.Text)
            {
                formHTML += $"""
                <div class="sm:col-span-4 my-4">
              <label for="{modelTypeProp.Name}" class="block text-sm font-medium leading-6 text-gray-900">{modelTypeProp.Name}</label>
              <div class="mt-2">
                <div class="flex rounded-md shadow-sm ring-1 ring-inset ring-gray-300 focus-within:ring-2 focus-within:ring-inset focus-within:ring-indigo-600 sm:max-w-md">
                  <input type="text" name="{modelTypeProp.Name}" id="{modelTypeProp.Name}" value="{modelTypeProp.GetValue(model)}" class="block w-full rounded-md border-0 p-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6">
                </div>
              </div>
              </div>
              """;
            }

            if (modelTypePropAttrsDataType.DataType == DataType.Password)
            {
                var valueExists = (modelTypeProp.GetValue(model) is not null) ? "ValueExists" : "";
                formHTML += $"""
                <div class="sm:col-span-4 my-4">
              <label for="{modelTypeProp.Name}" class="block text-sm font-medium leading-6 text-gray-900">{modelTypeProp.Name}</label>
              <div class="mt-2">
                <div class="flex rounded-md shadow-sm ring-1 ring-inset ring-gray-300 focus-within:ring-2 focus-within:ring-inset focus-within:ring-indigo-600 sm:max-w-md">
                  <input type="password" name="{modelTypeProp.Name}" id="{modelTypeProp.Name}" value="{valueExists}" class="block w-full rounded-md border-0 p-1.5 text-gray-900 shadow-sm ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-indigo-600 sm:text-sm sm:leading-6">
                </div>
              </div>
              </div>
              """;
            }
        }
        
        return formHTML; // output form html
    }
}