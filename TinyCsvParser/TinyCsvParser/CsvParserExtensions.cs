﻿// Copyright (c) Philipp Wagner. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser.Mapping;
using TinyCsvParser.Model;

namespace TinyCsvParser
{
    public static class CsvParserExtensions
    {
        public static IAsyncEnumerable<CsvMappingResult<TEntity>> ReadFromFileAsync<TEntity>(this CsvParser<TEntity> csvParser, string fileName, Encoding encoding)
            where TEntity : class, new()
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var lines = File
                .ReadLines(fileName, encoding)
                .Select((line, index) => new Row(index, line))
                .ToAsyncEnumerable();

            return csvParser.ParseAsync(lines);
        }

        public static IAsyncEnumerable<CsvMappingResult<TEntity>> ReadFromStringAsync<TEntity>(this CsvParser<TEntity> csvParser, CsvReaderOptions csvReaderOptions, string csvData)
            where TEntity : class, new()
        {
            var lines = csvData
                .Split(csvReaderOptions.NewLine, StringSplitOptions.None)
                .Select((line, index) => new Row(index, line))
                .ToAsyncEnumerable();

            return csvParser.ParseAsync(lines);
        }

        private static async IAsyncEnumerable<string> ReadLinesFromStreamAsync(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 1024, bool leaveOpen = false)
        {
            using var reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen);
            while (!reader.EndOfStream)
            {
                yield return await reader.ReadLineAsync();
            }
        }

        public static IAsyncEnumerable<CsvMappingResult<TEntity>> ReadFromStreamAsync<TEntity>(this CsvParser<TEntity> csvParser, Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks = false, int bufferSize = 1024, bool leaveOpen = false)
            where TEntity : class, new()
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var lines = ReadLinesFromStreamAsync(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
                .Select((line, index) => new Row(index, line));

            return csvParser.ParseAsync(lines);
        }
    }
}
