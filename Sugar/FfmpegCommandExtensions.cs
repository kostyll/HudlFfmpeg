﻿using System;
using System.Collections.Generic;
using System.Linq;
using Hudl.Ffmpeg.Command;
using Hudl.Ffmpeg.Common;
using Hudl.Ffmpeg.Filters.BaseTypes;
using Hudl.Ffmpeg.Resources;
using Hudl.Ffmpeg.Settings.BaseTypes;

namespace Hudl.Ffmpeg.Sugar
{
    public static class FfmpegCommandExtensions
    {
        public static void ValidateOutput(FfmpegCommand command, string fileName, SettingsCollection collection)
        {
            if (command.Owner == null)
            {
                throw new ArgumentException("Command must contain an owner before sugar is allowed.", "command");
            }

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (collection.Type != SettingsCollectionResourceType.Output)
            {
                throw new ArgumentException("Command output settings collection must be a type of Output.", "collection");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Command fileName cannot be null or empty.", "fileName");
            }

            if (command.Owner.CommandList.All(c => c.Id != command.Id))
            {
                throw new ArgumentException("Command must be added via CreateOutput or CreateResource first.", "command");
            }
        }
        public static FfmpegCommand WithOutput(this FfmpegCommand command, string fileName)
        {
            return command.WithOutput(fileName, SettingsCollection.ForOutput());
        }
        public static FfmpegCommand WithOutput(this FfmpegCommand command, List<string> files)
        {
            return command.WithOutput(files, SettingsCollection.ForOutput());
        }
        public static FfmpegCommand WithOutput(this FfmpegCommand command, string fileName, SettingsCollection collection)
        {
            ValidateOutput(command, fileName, collection);

            var resource = Resource.From(fileName);

            var commandOutput = CommandOutput.Create(resource);

            command.OutputManager.Add(commandOutput);

            return command;
        }
        public static FfmpegCommand WithOutput(this FfmpegCommand command, List<string> files, SettingsCollection collection)
        {
            if (files == null || files.Count == 0)
            {
                throw new ArgumentException("Files cannot be null or empty.", "files");
            }

            files.ForEach(fileName => command.WithOutput(fileName, collection));

            return command;
        }

        public static void ValidateInput(FfmpegCommand command, string fileName)
        {
            if (command.Owner == null)
            {
                throw new ArgumentException("Command must contain an owner before sugar is allowed.", "command");
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("Command fileName cannot be null or empty.", "fileName");
            }

            if (command.Owner.CommandList.All(c => c.Id != command.Id))
            {
                throw new ArgumentException("Command must be added via CreateOutput or CreateResource first.", "command");
            }
        }
        public static FfmpegCommand WithInput(this FfmpegCommand command, string fileName)
        {
            return command.WithInput(fileName, SettingsCollection.ForInput());
        }
        public static FfmpegCommand WithInput(this FfmpegCommand command, string fileName, SettingsCollection settings)
        {
            ValidateInput(command, fileName);

            var resource = Resource.From(fileName)
                                   .LoadMetadata();

            var commandResource = CommandResource.Create(resource, settings);

            command.ResourceManager.Add(commandResource);

            return command;
        }
        public static FfmpegCommand WithInputNoLoad(this FfmpegCommand command, string fileName)
        {
            ValidateInput(command, fileName);

            var resource = Resource.From(fileName);

            var commandResource = CommandResource.Create(resource);

            command.ResourceManager.Add(commandResource);

            return command;
        }
        public static FfmpegCommand WithInput(this FfmpegCommand command, List<string> files)
        {
            if (files == null || files.Count == 0)
            {
                throw new ArgumentException("Files cannot be null or empty.", "files");
            }

            files.ForEach(fileName => command.WithInput(fileName));

            return command;
        }

        public static CommandReceipt ResourceReceiptAt(this FfmpegCommand command, int index)
        {
            if (index < 0 || index >= command.Resources.Count)
            {
                throw new IndexOutOfRangeException();
            }

            return command.Resources[index].GetReceipt();
        }
        public static CommandReceipt LastInputReceipt(this FfmpegCommand command)
        {
            if (command.Resources.Count == 0) 
            {
                return null; 
            }

            return command.Resources[command.Resources.Count - 1].GetReceipt();
        }

        public static void ValidateRecipts(FfmpegCommand command, List<CommandReceipt> receipts)
        {
            if (command.Owner == null)
            {
                throw new ArgumentException("Command must contain an owner before sugar is allowed.", "command");
            }

            if (receipts == null)
            {
                throw new ArgumentNullException("receipts");
            }
        }
        public static CommandStage WithStreamAt(this FfmpegCommand command, int index)
        {
            var receipt = command.ResourceReceiptAt(index);
            return command.WithStreams(receipt); 
        }
        public static CommandStage WithStream(this FfmpegCommand command, CommandReceipt receipt)
        {
            var receiptList = new List<CommandReceipt>() { receipt };
            return command.WithStreams(receiptList);
        }
        public static CommandStage WithStreams(this FfmpegCommand command, params CommandReceipt[] receipts)
        {
            var receiptList = new List<CommandReceipt>(receipts);
            return command.WithStreams(receiptList);
        }
        public static CommandStage WithStreams(this FfmpegCommand command, List<CommandReceipt> receipts)
        {
            ValidateRecipts(command, receipts);

            return new CommandStage(command)
            {
                Receipts = receipts
            };
        }
        public static CommandStage WithStreamsFrom(this FfmpegCommand command, CommandResource resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }

            return command.WithStreams(resource.GetReceipt());
        }
        public static CommandStage WithStreamsFrom(this FfmpegCommand command, Filterchain filterchain)
        {
            if (filterchain == null)
            {
                throw new ArgumentNullException("filterchain");
            }

            return command.WithStreams(filterchain.GetReceipts());
        }

        public static CommandStage WithAllStreams(this FfmpegCommand command)
        {
            var receiptList = command.Resources.Select(r => r.GetReceipt()).ToList();
            return command.WithStreams(receiptList);
        }
    }
}