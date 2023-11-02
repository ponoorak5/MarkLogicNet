# MarkLogicNet

Simple Marklogic database client for .Net

## Instalation

```sh
Install-Package MarkLogicNet
```

### Usage

Add
```csharp
builder.Services.AddMarkLogicClient();
```

make sure you have configuration in you application.json
```json
 "MarkLogicClientConfiguration": {
    "Url": "http://url-to-marklogic-server",
    "Login": "username",
    "Password": "password",

    //Optional
    "Timeout": 30000
 }
```


Or
```csharp
builder.Services.AddMarkLogicClient(new MarkLogicClientConfiguration()
{
    Url = new Uri("http://http://url-to-marklogic-server"),
    Login = "username",
    Password = "password",
    //Optional
    Timeout = 30000
});
```

### Example

```csharp
    public class MarkLogicService
    {
        private readonly MarkLogicClient _markLogicClient;

        public MarkLogicService(MarkLogicClient markLogicClient)
        {
            _markLogicClient = markLogicClient;
        }

        public async Task<bool> Execute(Stream outputStream, string script, QueryLanguage language, CancellationToken token)
        {
            var result = await _markLogicClient.ExecuteScript(script, language, token);

            await using var writer = new StreamWriter(outputStream);
            writer.AutoFlush = true;

            if (result.HasError)
            {
                await writer.WriteLineAsync(result.ErrorMessage);
                return false;
            }

            using var reader = new MarkLogicStreamReader(result.Stream ?? throw new InvalidOperationException());
            while (await reader.ReadLineAsync() is { } res)
            {
                await writer.WriteLineAsync(res.Content);
            }

            return true;
        }
    }
 
```
