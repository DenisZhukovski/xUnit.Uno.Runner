using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotnet.Commands;
using XUnit.Runners.Core;
using Xunit.Uno.Runner.Extensions;
using Xunit.Uno.Runner.Navigation;

namespace Xunit.Uno.Runner
{
	public class MainViewModel : ObservableObject, INavigationAware
	{
        private readonly INavigator _navigator;
        private readonly ITestScanner _testsScanner;

        private CancellationToken? _progress;
        private readonly ICommands _commands;
        private ObservableCollection<TestCasesViewModel> _allTests = new();

        public MainViewModel(INavigator navigator, ITestScanner testsScanner, ICommands commands)
		{
            _navigator = navigator;
            _testsScanner = testsScanner;
            _commands = commands.Cached().Logged(Diagnostic);
		}

        private CancellationToken? Progress
        {
            get => _progress;
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    RunAllCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DiagnosticViewModel Diagnostic { get; } = new();
        
        public bool IsBusy => Progress != null;

        public ObservableCollection<TestCasesViewModel> AllTests
        {
            get => _allTests;
            private set => SetProperty(ref _allTests, value);
        }

        public IAsyncCommand CreditsCommand => _commands.AsyncCommand(
            () => _navigator.NavigateViewAsync<CreditsPage>(this)
        );
        
		public IAsyncCommand RunAllCommand => _commands.AsyncCommand(
            async token =>
            {
                try
                {
                    Progress = token;
                    Diagnostic.Clear();
                    Diagnostic.Write("Run Everything");
                    await AllTests.RunAsync(token);
                }
                finally
                {
                    Progress = null;
                }
            },
            () => !IsBusy
        );

        public ICommand TestCasesCommand => _commands.AsyncCommand<TestCasesViewModel?>(async viewModel =>
        {
            if (viewModel != null)
            {
               await _navigator.NavigateViewAsync<TestCasesPage>(this, data: viewModel);
            }
        });

        public IAsyncCommand ScanForTestsCommand => _commands.AsyncCommand(async token =>
        {
            try
            {
                Progress = token;
                AllTests = await _testsScanner
                    .Logged(Diagnostic)
                    .ToViewModels(
                        _navigator,
                        _commands,
                        token
                    );
            }
            finally
            {
                Progress = null;
            }
        });
        
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                ScanForTestsCommand.Execute();
            }
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
        }
    }
}
