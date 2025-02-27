//     _                _      _  ____   _                           _____
//    / \    _ __  ___ | |__  (_)/ ___| | |_  ___   __ _  _ __ ___  |  ___|__ _  _ __  _ __ ___
//   / _ \  | '__|/ __|| '_ \ | |\___ \ | __|/ _ \ / _` || '_ ` _ \ | |_  / _` || '__|| '_ ` _ \
//  / ___ \ | |  | (__ | | | || | ___) || |_|  __/| (_| || | | | | ||  _|| (_| || |   | | | | | |
// /_/   \_\|_|   \___||_| |_||_||____/  \__|\___| \__,_||_| |_| |_||_|   \__,_||_|   |_| |_| |_|
// |
// Copyright 2015-2019 Łukasz "JustArchi" Domeradzki
// Contact: JustArchi@JustArchi.net
// |
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// |
// http://www.apache.org/licenses/LICENSE-2.0
// |
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ArchiSteamFarm.Plugins;
using JetBrains.Annotations;

namespace ArchiSteamFarm {
	internal static class SharedInfo {
		internal const ulong ArchiSteamID = 76561198006963719;
		internal const string ArchivalLogFile = "log.{#}.txt";
		internal const string ArchivalLogsDirectory = "logs";
		internal const string ASF = nameof(ASF);
		internal const ulong ASFGroupSteamID = 103582791440160998;
		internal const string AssemblyDocumentation = AssemblyName + ".xml";
		internal const string AssemblyName = nameof(ArchiSteamFarm);
		internal const string ConfigDirectory = "config";
		internal const string DatabaseExtension = ".db";
		internal const string DebugDirectory = "debug";
		internal const string EnvironmentVariableCryptKey = ASF + "_CRYPTKEY";
		internal const string EnvironmentVariablePath = ASF + "_PATH";
		internal const string GithubReleaseURL = "https://api.github.com/repos/" + GithubRepo + "/releases"; // GitHub API is HTTPS only
		internal const string GithubRepo = "JustArchiNET/" + AssemblyName;
		internal const string GlobalConfigFileName = ASF + JsonConfigExtension;
		internal const string GlobalDatabaseFileName = ASF + DatabaseExtension;
		internal const string IPCConfigExtension = ".config";
		internal const string IPCConfigFile = nameof(IPC) + IPCConfigExtension;
		internal const string JsonConfigExtension = ".json";
		internal const string KeysExtension = ".keys";
		internal const string KeysUnusedExtension = ".unused";
		internal const string KeysUsedExtension = ".used";
		internal const string LicenseName = "Apache 2.0";
		internal const string LicenseURL = "http://www.apache.org/licenses/LICENSE-2.0";
		internal const string LogFile = "log.txt";
		internal const string MobileAuthenticatorExtension = ".maFile";
		internal const string PluginsDirectory = "plugins";
		internal const string ProjectURL = "https://github.com/" + GithubRepo;
		internal const string SentryHashExtension = ".bin";
		internal const string StatisticsServer = "asf.justarchi.net";
		internal const string UlongCompatibilityStringPrefix = "s_";
		internal const string UpdateDirectory = "_old";
		internal const string WebsiteDirectory = "www";

		internal static string HomeDirectory {
			get {
				// We're aiming to handle two possible cases here, classic publish and single-file publish
				// Firstly, we'll get the path to the binary that is running our code
				string binaryPath;

				using (Process process = Process.GetCurrentProcess()) {
					binaryPath = process.MainModule?.FileName;
				}

				if (string.IsNullOrEmpty(binaryPath)) {
					throw new ArgumentNullException(nameof(binaryPath));
				}

				// Now we need to check what that binary actually is
				return Path.GetFileNameWithoutExtension(binaryPath) switch {
					// This path goes to our own binary, so the user is using OS-specific build, single-file or not, we'll return path to location of that binary then
					AssemblyName => Path.GetDirectoryName(binaryPath),

					// This path goes to third-party binary, so the user is using our generic build, we'll return our base directory then
					"dotnet" => AppContext.BaseDirectory,
					"mono" => AppContext.BaseDirectory,

					// Unhandled case
					_ => throw new ArgumentOutOfRangeException(nameof(binaryPath))
				};
			}
		}

		[NotNull]
		internal static string ProgramIdentifier => PublicIdentifier + " V" + Version + " (" + BuildInfo.Variant + "/" + ModuleVersion + " | " + OS.Variant + ")";

		[NotNull]
		internal static string PublicIdentifier => AssemblyName + (BuildInfo.IsCustomBuild ? "-custom" : PluginsCore.HasActivePluginsLoaded ? "-modded" : "");

		[NotNull]
		internal static Version Version => Assembly.GetEntryAssembly()?.GetName().Version ?? throw new ArgumentNullException(nameof(Version));

		private static Guid ModuleVersion => Assembly.GetEntryAssembly()?.ManifestModule.ModuleVersionId ?? throw new ArgumentNullException(nameof(ModuleVersion));

		internal static class BuildInfo {
#if ASF_VARIANT_DOCKER
			internal static bool CanUpdate => false;

			[NotNull]
			internal static string Variant => "docker";
#elif ASF_VARIANT_GENERIC
			internal static bool CanUpdate => true;

			[NotNull]
			internal static string Variant => "generic";
#elif ASF_VARIANT_GENERIC_NETF
			internal static bool CanUpdate => true;

			[NotNull]
			internal static string Variant => "generic-netf";
#elif ASF_VARIANT_LINUX_ARM
			internal static bool CanUpdate => true;

			[NotNull]
			internal static string Variant => "linux-arm";
#elif ASF_VARIANT_LINUX_X64
			internal static bool CanUpdate => true;

			[NotNull]
			internal static string Variant => "linux-x64";
#elif ASF_VARIANT_OSX_X64
			internal static bool CanUpdate => true;

			[NotNull]
			internal static string Variant => "osx-x64";
#elif ASF_VARIANT_WIN_X64
			internal static bool CanUpdate => true;

			[NotNull]
			internal static string Variant => "win-x64";
#else
			internal static bool CanUpdate => false;

			[NotNull]
			internal static string Variant => SourceVariant;
#endif

			private const string SourceVariant = "source";

			internal static bool IsCustomBuild => Variant == SourceVariant;
		}
	}
}
