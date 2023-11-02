using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MarkLogicNet.Models;

namespace MarkLogicNet
{
    public class MarkLogicStreamReader : StreamReader
    {
        private string? _separator;

        public MarkLogicStreamReader(Stream stream) : base(stream)
        {
            base.ReadLine(); // First empty line
        }

        private async Task<string?> ReadLineException()
        {
            var line = await base.ReadLineAsync();
            if (line == null)
            {
                throw new NullReferenceException();
            }

            return line;
        }

        public new async Task<MarkLogicResult?> ReadLineAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(_separator))
                {
                    _separator = await ReadLineException();
                }

                var contentType = await ReadLineException();
                var result = new MarkLogicResult()
                {
                    ContentType = contentType
                };

                var part = new StringBuilder();
                bool isInContent = false;
                while (await base.ReadLineAsync() is { } line)
                {
                    if (_separator != null && line.StartsWith(_separator))
                    {
                        result.Content = part.ToString();
                        return result;
                    }

                    if (!string.IsNullOrEmpty(line))
                    {
                        if (!isInContent)
                        {
                            var sp = line.Split(':')!;
                            result.DataType.Add(sp[0], sp[1].TrimStart());
                            continue;
                        }
                    }
                    else
                    {
                        isInContent = true;
                        continue;
                    }
                    part.Append(line);
                }


            }
            catch (Exception)
            {
                return null;
            }

            return null;

        }
    }
}
