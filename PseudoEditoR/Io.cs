// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Handles everything with I/O.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using PseudoEditoR.Properties;
    using Newtonsoft.Json;

    /// <summary>
    /// Handles everything with I/O.
    /// </summary>
    /// <remarks>
    /// Loads local data important for the application and saves user generated content.
    /// </remarks>
    public static class Io
    {
        /// <summary>
        /// The file type for code.
        /// </summary>
        private const string FileTypeCode = "Rich Text Format|*.rtf";

        /// <summary>
        /// The <see cref="Directory"/> in which all saved user generated code is saved.
        /// </summary>
        private static readonly DirectoryInfo CodeDirectory = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + @"PseudoEditoR\code");

        /// <summary>
        /// The <see cref="Directory"/> which contains the serialized <see cref="ProgrammingLanguage"/>s as .plang files.
        /// </summary>
        private static readonly DirectoryInfo LanguagesDirectory = new DirectoryInfo(Environment.CurrentDirectory + @"\languages");

        /// <summary>
        /// The <see cref="Directory"/> which contains all localization languages as serialized <see cref="Dictionary{String, String}"/> in .lang files.
        /// </summary>
        private static readonly DirectoryInfo LocalizationDirectory = new DirectoryInfo(Environment.CurrentDirectory + @"\localization");

        /// <summary>
        /// Creates the custom files and folders.
        /// </summary>
        public static void CreateFiles()
        {
            if (!LanguagesDirectory.Exists)
            {
                LanguagesDirectory.Create();
            }

            if (!LocalizationDirectory.Exists)
            {
                LocalizationDirectory.Create();
            }

            ResourceSet resourceSet = Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry resource in resourceSet)
            {
                string content = Encoding.UTF8.GetString((byte[])resource.Value);
                var programmingLanguage = JsonConvert.DeserializeObject<ProgrammingLanguage>(content);
                if (programmingLanguage.Name == null)
                {
                    string path = LocalizationDirectory.FullName + Path.DirectorySeparatorChar + resource.Key + ".lang";
                    if (!File.Exists(path))
                    {
                        File.WriteAllText(path, content);
                    }
                }
                else
                {
                    string path = LanguagesDirectory.FullName + Path.DirectorySeparatorChar + resource.Key + ".plang";
                    if (!File.Exists(path))
                    {
                        File.WriteAllText(path, content);
                    }
                }
            }
        }

        /// <summary>
        /// Loads all <see cref="ProgrammingLanguage"/>-files and stores them in a list.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> containing an ICollection{ProgrammingLanguage} as <c>TResult</c>.
        /// </returns>
        public static async Task<ICollection<ProgrammingLanguage>> GetProgrammingLanguagesAsync()
        {
            if (!LanguagesDirectory.Exists || LanguagesDirectory.GetFiles().Length == 0)
            {
                MessageBox.Show("No programming language definitions found. Application will be exited.", "PseudoEditoR", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }

            return
                LanguagesDirectory.GetFiles("*.plang", SearchOption.AllDirectories)
                    .Select(file => JsonConvert.DeserializeObject<ProgrammingLanguage>(File.ReadAllText(file.FullName)))
                    .ToList();
        }

        /// <summary>
        /// Reads all <c>LocalizationLanguage</c>-files stores them in a list.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> containing an <see cref="System.Collections.ICollection"/> as <c>TResult</c>.
        /// </returns>
        public static async Task<ICollection<Dictionary<string, string>>> GetLocalizationLanguagesAsync()
        {
            if (!LocalizationDirectory.Exists || LocalizationDirectory.GetFiles().Length == 0)
            {
                MessageBox.Show("No localization language translations found. Application will be exited.", "PseudoEditoR", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }

            return
                LocalizationDirectory.GetFiles("*.lang", SearchOption.AllDirectories)
                    .Select(
                        file =>
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file.FullName)))
                    .ToList();
        }

        /// <summary>
        /// Saves the content of the RichTextBox as .rtf-File.
        /// </summary>
        /// <param name="richTextBox">
        /// The RichTextBox which holds the content.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The parameter was passed as null.
        /// </exception>
        public static async Task SaveRichTextBoxAsync(RichTextBox richTextBox)
        {
            if (richTextBox == null)
            {
                throw new ArgumentNullException("richTextBox");
            }

            var saveFileDialog = new System.Windows.Forms.SaveFileDialog
                                     {
                                         Filter = FileTypeCode,
                                         FileName = MainClass.CurrentLocalizationLanguage["code"],
                                         Title = MainClass.CurrentLocalizationLanguage["save"] + @" "
                                             + MainClass.CurrentLocalizationLanguage["code"],
                                         InitialDirectory = CodeDirectory.FullName
                                     };
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    richTextBox.SelectAll();
                    richTextBox.Selection.Save(stream, DataFormats.Rtf);
                }
            }
        }

        /// <summary>
        /// Loads the content of a .rtf-file in the RichTextBox.
        /// </summary>
        /// <param name="richTextBox">
        /// The RichTextBox where the content should be loaded to.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The parameter was passed as null.
        /// </exception>
        public static async Task LoadFileInRichTextBoxAsync(RichTextBox richTextBox)
        {
            if (richTextBox == null)
            {
                throw new ArgumentNullException("richTextBox");
            }

            var openFileDialog = new System.Windows.Forms.OpenFileDialog
                                     {
                                         Filter = FileTypeCode,
                                         Title = MainClass.CurrentLocalizationLanguage["load"] + @" "
                                             + MainClass.CurrentLocalizationLanguage["code"],
                                         InitialDirectory = CodeDirectory.FullName
                                     };
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (
                    var stream =
                        new MemoryStream(Encoding.Default.GetBytes(File.ReadAllText(openFileDialog.FileName))))
                {
                    richTextBox.SelectAll();
                    richTextBox.Selection.Load(stream, DataFormats.Rtf);
                }
            }
        }

        /// <summary>
        /// Prints the content of a <see cref="RichTextBox"/>.
        /// </summary>
        /// <param name="richTextBox">
        /// The <see cref="RichTextBox"/> which holds the content.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The parameters was passed as null.
        /// </exception>
        public static async Task PrintCodeAsync(RichTextBox richTextBox)
        {
            if (richTextBox == null)
            {
                throw new ArgumentNullException("richTextBox");
            }

            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                ////printDialog.PrintVisual(richTextBox, "PER PseudoCode");
                printDialog.PrintDocument(((IDocumentPaginatorSource)richTextBox.Document).DocumentPaginator, "PER PseudoCode");
            }
        }
    }
}
