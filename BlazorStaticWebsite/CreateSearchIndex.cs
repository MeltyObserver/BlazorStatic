using Markdig;
using Markdig.Syntax;
using Markdig.Extensions.Yaml;
using YamlDotNet.Serialization;
using System.Text.Json;
using System.Text;

namespace BlazorStaticWebsite;

internal static class CreateSearchIndex
{

    internal static void ParseBlogs()
    {
        string filePattern = "*.md";
        var path = Path.GetFullPath("./Content");
        var files = Directory.GetFiles(path, filePattern, SearchOption.AllDirectories);

        var mdPipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseYamlFrontMatter()
        .Build();
        JsonObject jsonIndex = new();

        for(int i = 0; i < files.Length; i++)
        {
            var file = files[i];
            var markdownContent = File.ReadAllText(file, Encoding.UTF8);
            var document = Markdown.Parse(markdownContent, mdPipeline);

            IDeserializer yamlDeserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
            var yamlBlock = document.Descendants<YamlFrontMatterBlock>().FirstOrDefault();

            if(yamlBlock is null) { continue; }

            string YamlString = yamlBlock.Lines.ToString();
            var metadata = yamlDeserializer.Deserialize<Dictionary<object, object>>(YamlString);

            jsonIndex.url.Add(file.Replace(path, "").TrimEnd(filePattern.ToCharArray()));
            jsonIndex.content[
                FormatMetaData(metadata)
            ] = i;
        }

        string jsonString = JsonSerializer.Serialize(jsonIndex);

        File.WriteAllText(Path.Combine(Path.GetFullPath("wwwroot"), "Index.json"), jsonString);
    }
    static string FormatMetaData(Dictionary<object, object> metadata)
    {
        string metaString = "";
        foreach(var value in metadata)
        {
            if(value.Value is List<object>)
            {
                metaString += string.Join(" ", value.Value as List<object>)+ "*-*";
                continue;
            }
            metaString += value.Value?.ToString()?.ToLower() + "*-*"; // random string to separate between values
        }
        return metaString;
    }
}


class JsonObject
{
    public List<string> url { get; set; } = new();
    public Dictionary<string, int> content { get; set; } = new();
}