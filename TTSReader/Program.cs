using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace TTSReader
{
	public static class Program
	{
		public const string punctuationChars = "?!:;";
		public const string whiteSpaceChars = "\n\r\t";
		public const string validMessageChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ .\"'1234567890,()*/-+&%$#@";

		private const string validSettingsChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ=.;";
		[STAThread]
		public static void Main()
		{
			CreateInputFileIfMissing();
			CreateSettingsFileIfMissing();
			CreateOutputFileIfMissing();

			string message = LoadMessage();
			Settings settings = LoadSettings();
			SpeakMessage(message, settings);
			if (settings.outputType == OutputType.File)
			{
				OpenOutputFile();
			}
		}

		public static void SpeakMessage(string message, Settings settings)
		{
			System.Speech.Synthesis.SpeechSynthesizer speechSynthesizer = ConvertSettings(settings);

			speechSynthesizer.Speak(message);
		}

		public static System.Speech.Synthesis.SpeechSynthesizer ConvertSettings(Settings settings)
		{
			System.Speech.Synthesis.SpeechSynthesizer speechSynthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
			speechSynthesizer.SelectVoiceByHints(ConvertSpeakerSex(settings.speakerSex), ConvertSpeakerAge(settings.speakerAge));
			speechSynthesizer.Rate = ConvertSpeechRate(settings.speechRate);
			switch (settings.outputType)
			{
				case OutputType.AudioDevice:
					speechSynthesizer.SetOutputToDefaultAudioDevice();
					break;
				case OutputType.File:
					speechSynthesizer.SetOutputToWaveFile(GetOutputFilePath());
					break;
				default:
					speechSynthesizer.SetOutputToDefaultAudioDevice();
					break;
			}
			return speechSynthesizer;
		}
		public static int ConvertSpeechRate(SpeechRate speechRate)
		{
			switch (speechRate)
			{
				case SpeechRate.Slowest:
					return -10;
				case SpeechRate.Slower:
					return -6;
				case SpeechRate.Slow:
					return -3;
				case SpeechRate.Normal:
					return 0;
				case SpeechRate.Fast:
					return 3;
				case SpeechRate.Faster:
					return 6;
				case SpeechRate.Fastest:
					return 10;
				default:
					return 0;
			}
		}
		public static System.Speech.Synthesis.VoiceGender ConvertSpeakerSex(SpeakerSex speakerSex)
		{
			switch (speakerSex)
			{
				case SpeakerSex.Male:
					return System.Speech.Synthesis.VoiceGender.Male;
				case SpeakerSex.Female:
					return System.Speech.Synthesis.VoiceGender.Female;
				case SpeakerSex.Neutral:
					return System.Speech.Synthesis.VoiceGender.Neutral;
				default:
					return System.Speech.Synthesis.VoiceGender.Neutral;
			}
		}
		public static System.Speech.Synthesis.VoiceAge ConvertSpeakerAge(SpeakerAge speakerAge)
		{
			switch (speakerAge)
			{
				case SpeakerAge.Child:
					return System.Speech.Synthesis.VoiceAge.Child;
				case SpeakerAge.Teen:
					return System.Speech.Synthesis.VoiceAge.Teen;
				case SpeakerAge.Adult:
					return System.Speech.Synthesis.VoiceAge.Adult;
				case SpeakerAge.Senior:
					return System.Speech.Synthesis.VoiceAge.Senior;
				default:
					return System.Speech.Synthesis.VoiceAge.Adult;
			}
		}

		public static void OpenInputFile()
		{
			CreateInputFileIfMissing();
			Process.Start(GetInputFilePath());
		}
		public static void OpenSettingsFile()
		{
			CreateSettingsFileIfMissing();
			Process.Start(GetSettingsFilePath());
		}
		public static void OpenOutputFile()
		{
			CreateOutputFileIfMissing();
			Process.Start(GetOutputFilePath());
		}

		public static void CreateInputFileIfMissing()
		{
			if (!File.Exists(GetInputFilePath()))
			{
				File.WriteAllText(GetInputFilePath(), "Thanks for installing Randomia Gaming's TTS Reader. To use this reader just paste the text you want read into the TTS Input.txt file.");
			}
		}
		public static void CreateSettingsFileIfMissing()
		{
			if (!File.Exists(GetSettingsFilePath()))
			{
				File.WriteAllText(GetSettingsFilePath(), Settings.DefaultSettings.Serialize());
			}
		}
		public static void CreateOutputFileIfMissing()
		{
			if (!File.Exists(GetOutputFilePath()))
			{
				File.WriteAllBytes(GetOutputFilePath(), new byte[0]);
			}
		}

		public static string GetInputFilePath()
		{
			return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + @"\TTSInput.txt";
		}
		public static string GetSettingsFilePath()
		{
			return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + @"\TTSSettings.txt";
		}
		public static string GetOutputFilePath()
		{
			return Path.GetDirectoryName(Assembly.GetCallingAssembly().Location) + @"\TTSOutput.wav";
		}

		public static Settings LoadSettings()
		{
			CreateSettingsFileIfMissing();
			Settings output = new Settings(File.ReadAllText(GetSettingsFilePath()));
			return output;
		}
		public static string LoadMessage()
		{
			CreateInputFileIfMissing();
			return CleanMessage(File.ReadAllText(GetInputFilePath()));
		}

		public static string SerializeSettings(Settings settings)
		{
			List<string> statements = new List<string>();
			switch (settings.speakerAge)
			{
				case SpeakerAge.Child:
					statements.Add("speakerAge = SpeakerAge.Child");
					break;
				case SpeakerAge.Teen:
					statements.Add("speakerAge = SpeakerAge.Teen");
					break;
				case SpeakerAge.Adult:
					statements.Add("speakerAge = SpeakerAge.Adult");
					break;
				case SpeakerAge.Senior:
					statements.Add("speakerAge = SpeakerAge.Senior");
					break;
				default:
					statements.Add("speakerAge = SpeakerAge.Adult");
					break;
			}
			switch (settings.speakerSex)
			{
				case SpeakerSex.Male:
					statements.Add("speakerSex = SpeakerSex.Male");
					break;
				case SpeakerSex.Female:
					statements.Add("speakerSex = SpeakerSex.Female");
					break;
				case SpeakerSex.Neutral:
					statements.Add("speakerSex = SpeakerSex.Neutral");
					break;
				default:
					statements.Add("speakerSex = SpeakerSex.Neutral");
					break;
			}
			switch (settings.speechRate)
			{
				case SpeechRate.Slowest:
					statements.Add("speechRate = SpeechRate.Slowest");
					break;
				case SpeechRate.Slower:
					statements.Add("speechRate = SpeechRate.Slower");
					break;
				case SpeechRate.Slow:
					statements.Add("speechRate = SpeechRate.Slow");
					break;
				case SpeechRate.Normal:
					statements.Add("speechRate = SpeechRate.Normal");
					break;
				case SpeechRate.Fast:
					statements.Add("speechRate = SpeechRate.Fast");
					break;
				case SpeechRate.Faster:
					statements.Add("speechRate = SpeechRate.Faster");
					break;
				case SpeechRate.Fastest:
					statements.Add("speechRate = SpeechRate.Fastest");
					break;
				default:
					statements.Add("speechRate = SpeechRate.Normal");
					break;
			}
			switch (settings.outputType)
			{
				case OutputType.AudioDevice:
					statements.Add("outputType = OutputType.AudioDevice");
					break;
				case OutputType.File:
					statements.Add("outputType = OutputType.File");
					break;
				default:
					statements.Add("outputType = OutputType.AudioDevice");
					break;
			}
			string output = "";
			for (int i = 0; i < statements.Count; i++)
			{
				output += statements[i] + ";";
				if (i < statements.Count - 1)
				{
					output += "\n";
				}
			}
			return output;
		}
		public static Settings DeserializeSettings(string settings)
		{
			settings = CleanSettings(settings);
			List<string> statementStrings = SliceStatementStrings(settings);
			List<Statement> statements = DeserializeStatements(statementStrings);
			Settings output = Settings.DefaultSettings;
			foreach (Statement statement in statements)
			{
				switch (statement.targetVariable)
				{
					case "speakerAge":
						switch (statement.targetValue)
						{
							case "SpeakerAge.Child":
								output.speakerAge = SpeakerAge.Child;
								break;
							case "SpeakerAge.Teen":
								output.speakerAge = SpeakerAge.Teen;
								break;
							case "SpeakerAge.Adult":
								output.speakerAge = SpeakerAge.Adult;
								break;
							case "SpeakerAge.Senior":
								output.speakerAge = SpeakerAge.Senior;
								break;
						}
						break;
					case "speakerSex":
						switch (statement.targetValue)
						{
							case "SpeakerSex.Male":
								output.speakerSex = SpeakerSex.Male;
								break;
							case "SpeakerSex.Female":
								output.speakerSex = SpeakerSex.Female;
								break;
							case "SpeakerSex.Neutral":
								output.speakerSex = SpeakerSex.Neutral;
								break;
						}
						break;
					case "speechRate":
						switch (statement.targetValue)
						{
							case "SpeechRate.Slowest":
								output.speechRate = SpeechRate.Slowest;
								break;
							case "SpeechRate.Slower":
								output.speechRate = SpeechRate.Slower;
								break;
							case "SpeechRate.Slow":
								output.speechRate = SpeechRate.Slow;
								break;
							case "SpeechRate.Normal":
								output.speechRate = SpeechRate.Normal;
								break;
							case "SpeechRate.Fast":
								output.speechRate = SpeechRate.Fast;
								break;
							case "SpeechRate.Faster":
								output.speechRate = SpeechRate.Faster;
								break;
							case "SpeechRate.Fastest":
								output.speechRate = SpeechRate.Fastest;
								break;
						}
						break;
					case "outputType":
						switch (statement.targetValue)
						{
							case "OutputType.File":
								output.outputType = OutputType.File;
								break;
							case "OutputType.AudioDevice":
								output.outputType = OutputType.AudioDevice;
								break;
						}
						break;
				}
			}
			return output;
		}
		public static List<Statement> DeserializeStatements(List<string> statements)
		{
			List<Statement> output = new List<Statement>();

			foreach (string statement in statements)
			{
				try
				{
					output.Add(DeserializeStatement(statement));
				}
				catch
				{

				}
			}

			return output;
		}
		public static Statement DeserializeStatement(string statement)
		{
			string targetVariable = "";
			string targetValue = "";

			bool foundEquals = false;
			for (int i = 0; i < statement.Length; i++)
			{
				if (statement[i] == '=')
				{
					if (foundEquals)
					{
						throw new ArgumentException();
					}
					else
					{
						foundEquals = true;
					}
				}
				else
				{
					if (foundEquals)
					{
						targetValue += statement[i];
					}
					else
					{
						targetVariable += statement[i];
					}
				}
			}

			if (targetVariable == "" || targetValue == "")
			{
				throw new ArgumentException();
			}

			return new Statement(targetVariable, targetValue);
		}

		public static List<string> SliceStatementStrings(string settings)
		{
			List<string> output = new List<string>();
			string currentStatement = "";

			for (int i = 0; i < settings.Length; i++)
			{
				if (settings[i] == ';')
				{
					output.Add(currentStatement);
					currentStatement = "";
				}
				else
				{
					currentStatement += settings[i];
				}
			}

			if (currentStatement != "")
			{
				output.Add(currentStatement);
			}

			return output;
		}

		public static string CleanSettings(string settings)
		{
			string output = "";
			for (int i = 0; i < settings.Length; i++)
			{
				if (validSettingsChars.Contains(settings[i].ToString()))
				{
					output += settings[i];
				}
				else
				{

				}
			}
			return output;
		}
		public static string CleanMessage(string message)
		{
			string stage1 = message;

			string stage2 = "";
			for (int i = 0; i < stage1.Length; i++)
			{
				if (punctuationChars.Contains(stage1[i].ToString()))
				{
					stage2 += ".";
				}
				else
				{
					stage2 += stage1[i];
				}
			}

			string stage3 = "";
			for (int i = 0; i < stage2.Length; i++)
			{
				if (whiteSpaceChars.Contains(stage2[i].ToString()))
				{
					stage3 += " ";
				}
				else
				{
					stage3 += stage2[i];
				}
			}

			string stage4 = "";
			for (int i = 0; i < stage3.Length; i++)
			{
				if (validMessageChars.Contains(stage3[i].ToString()))
				{
					stage4 += stage3[i];
				}
				else
				{

				}
			}

			return stage4;
		}
	}
	public struct Settings
	{
		public static Settings DefaultSettings
		{
			get
			{
				return new Settings(SpeakerAge.Adult, SpeakerSex.Neutral, SpeechRate.Normal, OutputType.AudioDevice);
			}
		}
		public SpeakerAge speakerAge;
		public SpeakerSex speakerSex;
		public SpeechRate speechRate;
		public OutputType outputType;
		public Settings(SpeakerAge speakerAge, SpeakerSex speakerSex, SpeechRate speechRate, OutputType outputType)
		{
			this.speakerAge = speakerAge;
			this.speakerSex = speakerSex;
			this.speechRate = speechRate;
			this.outputType = outputType;
		}
		public Settings(string settings)
		{
			Settings temp = Program.DeserializeSettings(settings);
			speakerAge = temp.speakerAge;
			speakerSex = temp.speakerSex;
			speechRate = temp.speechRate;
			outputType = temp.outputType;
		}
		public string Serialize()
		{
			return Program.SerializeSettings(this);
		}
	}
	public enum SpeakerAge { Child, Teen, Adult, Senior };
	public enum SpeakerSex { Male, Female, Neutral };
	public enum SpeechRate { Slowest, Slower, Slow, Normal, Fast, Faster, Fastest };
	public enum OutputType { File, AudioDevice };
	public struct Statement
	{
		public string targetVariable;
		public string targetValue;
		public Statement(string targetVariable, string targetValue)
		{
			if (targetVariable == null || targetValue == null)
			{
				throw new NullReferenceException();
			}
			if (targetVariable == "" || targetValue == "")
			{
				throw new ArgumentException();
			}
			this.targetVariable = targetVariable;
			this.targetValue = targetValue;
		}
	}
}
