// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The main class of the project, controlling important parts of application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PseudoEditoR
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using PseudoEditoR.MistakeSearch;
    using PseudoEditoR.UserInteraction;
    using PseudoEditoR.WordRecognition;

    /// <summary>
    /// The main class of the project, controlling important parts of application.
    /// </summary>
    /// <remarks>
    /// Starts and ends important functions for the application.
    /// </remarks>
    public static class MainClass
    {
        /// <summary>
        /// The currently used <see cref="ProgrammingLanguage"/>.
        /// </summary>
        private static ProgrammingLanguage currentProgrammingLanguage;

        /// <summary>
        /// The currently used localization language.
        /// </summary>
        private static Dictionary<string, string> currentLocalizationLanguage;

        /// <summary>
        /// A <see cref="Task"/> used to control <see cref="FacilitateCoding.HighlightSyntaxAsync"/>.
        /// </summary>
        private static Task highlightSyntaxTask;

        /// <summary>
        /// A <see cref="Task"/> used to control <see cref="FacilitateCoding.PseudoSenseAsync"/>.
        /// </summary>
        private static Task pseudoSenseTask;

        /// <summary>
        /// A <see cref="Task"/> used to control <see cref="MistakeEngine.MistakeSearchAsync"/>.
        /// </summary>
        private static Task errorSearchTask;

        /// <summary>
        /// A <see cref="CancellationTokenSource"/> holding a <see cref="CancellationToken"/> used to cancel <see cref="Task"/>s in the <see cref="MainClass"/>.
        /// </summary>
        private static CancellationTokenSource cancellationToken;

        /// <summary>
        /// Gets the static instance of <see cref="MainWindow"/> used for the application.
        /// </summary>
        /// <seealso cref="MainWindow"/>
        public static MainWindow MainWindow { get; private set; }

        /// <summary>
        /// Gets or sets the currently used <see cref="ProgrammingLanguage"/> of the application. 
        /// Setting this properties value will automatically change everything according to the new <see cref="ProgrammingLanguage"/>.
        /// </summary>
        /// <seealso cref="ProgrammingLanguage"/>
        public static ProgrammingLanguage CurrentProgrammingLanguage
        {
            get
            {
                return currentProgrammingLanguage;
            }

            set
            {
                ChangeProgrammingLanguage(value, TaskScheduler.FromCurrentSynchronizationContext());
            } 
        }

        /// <summary>
        /// Gets or sets the currently used localization language of the application.
        /// Setting this properties value will automatically localize everything in the application according to the new localization language.
        /// </summary>
        public static Dictionary<string, string> CurrentLocalizationLanguage
        {
            get
            {
                return currentLocalizationLanguage;
            }

            set
            {
                ChangeLocalizationLanguage(value, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="MainClass.CodeAlteredAsync"/> should be executed or not.
        /// </summary>
        public static bool NoTextChangedPlox { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="List{Dictionary}"/> containing all usable localization languages.
        /// </summary>
        private static List<Dictionary<string, string>> LocalizationLanguages { get; set; } 

        /// <summary>
        /// Gets or sets a <see cref="List{ProgrammingLanguage}"/> containing all usable <see cref="ProgrammingLanguage"/>s.
        /// </summary>
        private static List<ProgrammingLanguage> ProgrammingLanguages { get; set; }

        /// <summary>
        /// Initializes important members of this application.
        /// </summary>
        /// <param name="mainWindow">
        /// A static instance of <see cref="MainWindow"/> which will be provided for other classes.
        /// </param>
        /// <param name="scheduler">
        /// A <see cref="TaskScheduler"/> used to start the start <see cref="Task"/>s.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The parameter was passed as null.
        /// </exception>
        public static async void StartAsync(MainWindow mainWindow, TaskScheduler scheduler)
        {
            if (mainWindow == null)
            {
                throw new ArgumentNullException("mainWindow");
            }

            Io.CreateFiles();

            var loadFilesTask = LoadFilesAsync();
            MainWindow = mainWindow;

            // ToDo: C# 6.0 auto-initialization of properties
            NoTextChangedPlox = false;

            cancellationToken = new CancellationTokenSource();
            highlightSyntaxTask = new Task(() => FacilitateCoding.HighlightSyntaxAsync(RecognitionEngine.AllWordsInCode, CurrentProgrammingLanguage), CancellationToken.None, TaskCreationOptions.LongRunning);
            pseudoSenseTask = new Task(() => FacilitateCoding.PseudoSenseAsync(RecognitionEngine.PreviousWord, RecognitionEngine.AllWordsInCode, CurrentProgrammingLanguage, MainWindow.CodeListBox), CancellationToken.None, TaskCreationOptions.LongRunning);
            errorSearchTask =
                new Task(
                    () =>
                    MistakeEngine.MistakeSearchAsync(
                        RecognitionEngine.AllWordsInCode,
                        mainWindow.ErrorListView,
                        TaskScheduler.FromCurrentSynchronizationContext()),
                    CancellationToken.None,
                    TaskCreationOptions.LongRunning);

            await Task.Factory.StartNew(() => Gui.FillProgrammingLanguageComboBoxAsync(ProgrammingLanguages, MainWindow.ProgrammingLanguageComboBox), CancellationToken.None, TaskCreationOptions.None, scheduler);
            await loadFilesTask.ContinueWith(task => Gui.FillLocalizationLanguageComboBoxAsync(LocalizationLanguages, MainWindow.LocalizationLanguageComboBox), scheduler);
            MainWindow.LocalizationLanguageComboBox.SelectedItem = currentLocalizationLanguage["language"];
            MainWindow.ProgrammingLanguageComboBox.SelectedItem = currentProgrammingLanguage.Name;
            mainWindow.Show();
            mainWindow.CodeTextBox.Focus();
        }

        /// <summary>
        /// Starts the fast operations to be executed at every user interaction with the <see cref="RichTextBox"/>.
        /// </summary>
        /// <param name="richTextBox">
        /// The <see cref="RichTextBox"/> which text was changed.
        /// </param>
        /// <param name="eventArgs">
        /// The <see cref="TextChangedEventArgs"/> containing specific information about the user interaction.
        /// </param>
        /// <param name="scheduler">
        /// The <see cref="TaskScheduler"/> containing GUI-context.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static async Task CodeAlteredAsync(RichTextBox richTextBox, TextChangedEventArgs eventArgs, TaskScheduler scheduler)
        {
            if (richTextBox == null)
            {
                throw new ArgumentNullException("richTextBox");
            }

            if (eventArgs == null)
            {
                throw new ArgumentNullException("eventArgs");
            }

            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }

            if (NoTextChangedPlox)
            {
                return;
            }

            var text =
                new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text.Replace(
                    " ",
                    string.Empty);
            if (richTextBox.Document == null || text == "\r\n")
            {
                RecognitionEngine.AllWordsInCode.Clear();
                MistakeEngine.Mistakes.Clear();
                MainWindow.ErrorListView.Items.Clear();
                return;
            }

            TextChange textChange = null;
            var textChanges = eventArgs.Changes;
            if (textChanges != null && textChanges.Count > 0)
            {
                textChange = textChanges.First();
            }

            if (textChange == null || (textChange.AddedLength <= 0 && textChange.RemovedLength <= 0))
            {
                return;
            }
            
            IEnumerable<Word> changedWords = RecognitionEngine.RecognizeWordsInCode(richTextBox.CaretPosition, CurrentProgrammingLanguage).Result;

            var newWord = changedWords.First();

            if (newWord.Content == string.Empty)
            {
                await FacilitateCoding.PseudoSenseAsync(newWord, RecognitionEngine.AllWordsInCode, CurrentProgrammingLanguage, MainWindow.CodeListBox);
                return;
            }

            switch (pseudoSenseTask.Status)
            {
                case TaskStatus.Created:
                    pseudoSenseTask.Start(scheduler);
                    break;
                case TaskStatus.RanToCompletion:
                case TaskStatus.Faulted:
                    pseudoSenseTask.Dispose();
                    pseudoSenseTask = new Task(
                        () => FacilitateCoding.PseudoSenseAsync(newWord, RecognitionEngine.AllWordsInCode, CurrentProgrammingLanguage, MainWindow.CodeListBox),
                        cancellationToken.Token,
                        TaskCreationOptions.LongRunning);
                    pseudoSenseTask.Start(scheduler);
                    break;
            }
        }

        /// <summary>
        /// Executes all operations to be made before closing and then effectively closes the application.
        /// </summary>
        public static void ExitApplication()
        {
            Properties.Settings.Default.LastUsedLocalizationLanguage = (string)MainWindow.LocalizationLanguageComboBox.SelectedItem;
            Properties.Settings.Default.LastUsedProgrammingLanguage = (string)MainWindow.ProgrammingLanguageComboBox.SelectedItem;
            Properties.Settings.Default.Save();
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// Starts the performance intensive <see cref="Task"/>s which round up <see cref="MainClass.CodeAlteredAsync"/>.
        /// </summary>
        /// <param name="scheduler">
        /// The <see cref="TaskScheduler"/> containing GUI-context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The parameter was passed as null.
        /// </exception>
        public static async void StartPerformanceIntensiveCodeAlteredTasksAsync(TaskScheduler scheduler)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }

            switch (highlightSyntaxTask.Status)
            {
                case TaskStatus.Created:
                    highlightSyntaxTask.Start(scheduler);
                    break;
                case TaskStatus.RanToCompletion:
                case TaskStatus.Canceled:
                case TaskStatus.Faulted:
                    highlightSyntaxTask.Dispose();
                    highlightSyntaxTask =
                        new Task(
                            () => FacilitateCoding.HighlightSyntaxAsync(RecognitionEngine.AllWordsInCode, CurrentProgrammingLanguage),
                            cancellationToken.Token,
                            TaskCreationOptions.LongRunning);
                    highlightSyntaxTask.Start(scheduler);
                    break;
            }

            switch (errorSearchTask.Status)
            {
                case TaskStatus.Created:
                    errorSearchTask.Start(scheduler);
                    break;
                case TaskStatus.RanToCompletion:
                case TaskStatus.Faulted:
                    errorSearchTask.Dispose();
                    errorSearchTask = new Task(
                        () => MistakeEngine.MistakeSearchAsync(RecognitionEngine.AllWordsInCode, MainWindow.ErrorListView, scheduler),
                        cancellationToken.Token,
                        TaskCreationOptions.LongRunning);
                    errorSearchTask.Start(scheduler);
                    break;
            }

            try
            {
                highlightSyntaxTask.Wait();
            }
            catch (AggregateException)
            {
                if (highlightSyntaxTask.Exception != null)
                {
                    Console.WriteLine(@"AggregateException thrown by {0}: {1}", highlightSyntaxTask.Id, highlightSyntaxTask.Exception.Message);
                    foreach (var innerException in highlightSyntaxTask.Exception.InnerExceptions)
                    {
                        Console.WriteLine(@"InnerException:{0}", innerException.Message);
                    }
                }
            }

            try
            {
                errorSearchTask.Wait();
            }
            catch (AggregateException)
            {
                if (errorSearchTask.Exception != null)
                {
                    Console.WriteLine(@"AggregateException thrown by {0}: {1}", errorSearchTask.Id, errorSearchTask.Exception.Message);
                    foreach (var innerException in errorSearchTask.Exception.InnerExceptions)
                    {
                        Console.WriteLine(@"InnerException:{0}", innerException.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Changes the currently used localization language and localizes everything according to the new localization language.
        /// </summary>
        /// <param name="localizationLanguage">
        /// The localization language to be changed to as <see cref="Dictionary{String, String}"/>.
        /// </param>
        /// <param name="scheduler">
        /// The <see cref="TaskScheduler"/> containing GUI-context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        private static void ChangeLocalizationLanguage(Dictionary<string, string> localizationLanguage, TaskScheduler scheduler)
        {
            if (localizationLanguage == null || scheduler == null)
            {
                throw new ArgumentNullException();
            }

            currentLocalizationLanguage = localizationLanguage;

            Task.Factory.StartNew(() => LocalizeControlsAsync(), CancellationToken.None, TaskCreationOptions.None, scheduler);
        }

        /// <summary>
        /// Changes the currently used localization language by the new localization languages name and localizes everything according to the new localization language.
        /// </summary>
        /// <param name="localizationLanguage">
        /// The name of the localization language to be changed to.
        /// </param>
        /// <param name="scheduler">
        /// The <see cref="TaskScheduler"/> containing GUI-context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static void ChangeLocalizationLanguage(string localizationLanguage, TaskScheduler scheduler)
        {
            if (localizationLanguage == null || scheduler == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var item in LocalizationLanguages.Where(item => item["language"] == localizationLanguage))
            {
                CurrentLocalizationLanguage = item;
            }

            Task.Factory.StartNew(() => LocalizeControlsAsync(), CancellationToken.None, TaskCreationOptions.None, scheduler);
        }

        /// <summary>
        /// Changes the currently used <see cref="ProgrammingLanguage"/> and everything code specific according to the new <see cref="ProgrammingLanguage"/>.
        /// </summary>
        /// <param name="programmingLanguage">
        /// The <see cref="ProgrammingLanguage"/> to be changed to.
        /// </param>
        /// <param name="scheduler">
        /// The <see cref="TaskScheduler"/> containing GUI-context.
        /// </param>
        private static void ChangeProgrammingLanguage(ProgrammingLanguage programmingLanguage, TaskScheduler scheduler)
        {
            if (programmingLanguage == null || scheduler == null)
            {
                throw new ArgumentNullException();
            }

            currentProgrammingLanguage = programmingLanguage;
            cancellationToken.Cancel();
            Task.Factory.StartNew(() => RecognitionEngine.UpdateCodeAsync(currentProgrammingLanguage), CancellationToken.None, TaskCreationOptions.None, scheduler);
            LocalizeMistakes();
            StartPerformanceIntensiveCodeAlteredTasksAsync(TaskScheduler.FromCurrentSynchronizationContext());
            cancellationToken = new CancellationTokenSource();
        }

        /// <summary>
        /// Changes the currently used <see cref="ProgrammingLanguage"/> by the new <see cref="ProgrammingLanguage"/>s name and everything code specific according to the new <see cref="ProgrammingLanguage"/>.
        /// </summary>
        /// <param name="programmingLanguage">
        /// The name of the <see cref="ProgrammingLanguage"/> to be changed to.
        /// </param>
        /// <param name="scheduler">
        /// The <see cref="TaskScheduler"/> containing GUI-context.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// One or all of the parameters was/ were passed as null.
        /// </exception>
        public static void ChangeProgrammingLanguage(string programmingLanguage, TaskScheduler scheduler)
        {
            if (programmingLanguage == null || scheduler == null)
            {
                throw new ArgumentNullException();
            }

            foreach (var item in ProgrammingLanguages.Where(item => item.Name == programmingLanguage))
            {
                currentProgrammingLanguage = item;
            }

            cancellationToken.Cancel();
            Task.Factory.StartNew(() => RecognitionEngine.UpdateCodeAsync(currentProgrammingLanguage), CancellationToken.None, TaskCreationOptions.None, scheduler);
            LocalizeMistakes();
            StartPerformanceIntensiveCodeAlteredTasksAsync(TaskScheduler.FromCurrentSynchronizationContext());
            cancellationToken = new CancellationTokenSource();
        }

        /// <summary>
        /// Loads the local <see cref="File"/>s representing localization languages and <see cref="ProgrammingLanguages"/> and sets dependent fields to their value.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task LoadFilesAsync()
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            var localizationLanguages = await Io.GetLocalizationLanguagesAsync();
            LocalizationLanguages = localizationLanguages.ToList();
            if (Properties.Settings.Default.LastUsedLocalizationLanguage == string.Empty)
            {
                ChangeLocalizationLanguage(localizationLanguages.First(), scheduler);
            }
            else
            {
                ChangeLocalizationLanguage(Properties.Settings.Default.LastUsedLocalizationLanguage, scheduler);
            }

            var programmingLanguages = await Io.GetProgrammingLanguagesAsync();
            ProgrammingLanguages = programmingLanguages.ToList();
            if (Properties.Settings.Default.LastUsedProgrammingLanguage == string.Empty)
            {
                ChangeProgrammingLanguage(programmingLanguages.First(), scheduler);
            }
            else
            {
                ChangeProgrammingLanguage(Properties.Settings.Default.LastUsedProgrammingLanguage, scheduler);
            }
        }

        /// <summary>
        /// Localizes the <see cref="Control"/>s of the whole application.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task LocalizeControlsAsync()
        {
            var gridView = new GridView();

            var category = new GridViewColumn { DisplayMemberBinding = new Binding("Type") };
            var categoryHeader = new GridViewColumnHeader { Content = CurrentLocalizationLanguage["category"] };
            categoryHeader.Click += MainWindow.GridViewColumnHeader_Clicked;
            category.Header = categoryHeader;
            gridView.Columns.Add(category);

            var standardSequence = new GridViewColumn { DisplayMemberBinding = new Binding("StandardSequence") };
            var standardSequenceHeader = new GridViewColumnHeader { Content = CurrentLocalizationLanguage["standardSequence"] };
            standardSequenceHeader.Click += MainWindow.GridViewColumnHeader_Clicked;
            standardSequence.Header = standardSequenceHeader;
            gridView.Columns.Add(standardSequence);

            var description = new GridViewColumn { DisplayMemberBinding = new Binding("Description") };
            var descriptionHeader = new GridViewColumnHeader
                                                         {
                                                             Content = CurrentLocalizationLanguage["description"],
                                                             Width = MainWindow.ActualWidth - (MainWindow.ActualWidth / 3.2)
                                                         };
            descriptionHeader.Click += MainWindow.GridViewColumnHeader_Clicked;
            description.Header = descriptionHeader;
            gridView.Columns.Add(description);

            var line = new GridViewColumn { DisplayMemberBinding = new Binding("Line") };
            var lineHeader = new GridViewColumnHeader { Content = CurrentLocalizationLanguage["line"] };
            lineHeader.Click += MainWindow.GridViewColumnHeader_Clicked;
            line.Header = lineHeader;
            gridView.Columns.Add(line);

            var column = new GridViewColumn { DisplayMemberBinding = new Binding("Column") };
            var columnHeader = new GridViewColumnHeader { Content = CurrentLocalizationLanguage["column"] };
            columnHeader.Click += MainWindow.GridViewColumnHeader_Clicked;
            column.Header = columnHeader;
            gridView.Columns.Add(column);

            MainWindow.ErrorListView.View = gridView;

            MainWindow.CodeGroupBox.Header = CurrentLocalizationLanguage["code"];
            MainWindow.ErrorGroupBox.Header = CurrentLocalizationLanguage["error"];
            MainWindow.IoGroupBox.Header = CurrentLocalizationLanguage["io"];
            MainWindow.IoLoadButton.Content = CurrentLocalizationLanguage["load"] + "...";
            MainWindow.IoSaveButton.Content = CurrentLocalizationLanguage["save"] + "...";
            MainWindow.IoPrintButton.Content = CurrentLocalizationLanguage["print"] + "...";

            LocalizeMistakes();
        }

        /// <summary>
        /// Localizes the mistakes for the current <see cref="ProgrammingLanguage"/> according to the currently used localization language.
        /// </summary>
        private static void LocalizeMistakes()
        {
            if (CurrentProgrammingLanguage.MistakeDescriptions == null)
            {
                CurrentProgrammingLanguage.MistakeDescriptions = new Dictionary<string, string>
                                                                     {
                                                                         {
                                                                             "noDefinition",
                                                                             CurrentLocalizationLanguage["noDefinition"]
                                                                         },
                                                                         {
                                                                             "noDefinitionDesc",
                                                                             CurrentLocalizationLanguage["noDefinitionDesc"]
                                                                         }
                                                                     };
            }
            else
            {
                CurrentProgrammingLanguage.MistakeDescriptions["noDefinition"] = CurrentLocalizationLanguage["noDefinition"];
                CurrentProgrammingLanguage.MistakeDescriptions["noDefinitionDesc"] = CurrentLocalizationLanguage["noDefinitionDesc"];
            }
        }
    }
}
