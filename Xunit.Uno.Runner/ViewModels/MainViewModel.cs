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

        private CancellationToken? _progressCancelToken;
        private readonly ICommands _commands;
        private ObservableCollection<TestCasesViewModel> _testAssemblies = new();

        public MainViewModel(INavigator navigator, ITestScanner testsScanner, ICommands commands)
		{
            _navigator = navigator;
            _testsScanner = testsScanner;
            _commands = commands.Cached().Logged(Diagnostic, false);
		}

        private CancellationToken? ProgressCancelToken
        {
            get => _progressCancelToken;
            set
            {
                if (_progressCancelToken != value)
                {
                    _progressCancelToken = value;
                    RunEverythingCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DiagnosticViewModel Diagnostic { get; } = new();
        
        public bool IsBusy => ProgressCancelToken != null;

        public ObservableCollection<TestCasesViewModel> TestAssemblies
        {
            get => _testAssemblies;
            private set => SetProperty(ref _testAssemblies, value);
        }

        public IAsyncCommand CreditsCommand => _commands.AsyncCommand(
            () => _navigator.NavigateViewAsync<CreditsPage>(this)
        );
        
		public IAsyncCommand RunEverythingCommand => _commands.AsyncCommand(
            async token =>
            {
                try
                {
                    ProgressCancelToken = token;
                    Diagnostic.Clear();
                    Diagnostic.Write("Run Everything");
                    await TestAssemblies.RunAsync(token);
                }
                finally
                {
                    ProgressCancelToken = null;
                }
            },
            () => !IsBusy
        );

        public ICommand NavigateToTestAssemblyCommand => _commands.AsyncCommand<TestCasesViewModel?>(async viewModel =>
        {
            if (viewModel != null)
            {
               // await _navigator.NavigateAsync(PageType.AssemblyTestList, viewModel);
            }
           
        });

        public IAsyncCommand ScanAssembliesForTests => _commands.AsyncCommand(async token =>
        {
            try
            {
                ProgressCancelToken = token;
                TestAssemblies = await _testsScanner
                    .Logged(Diagnostic)
                    .ToViewModels(
                        _navigator,
                        _commands,
                        token
                    );
            }
            finally
            {
                ProgressCancelToken = null;
            }
        });
        
        public void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                ScanAssembliesForTests.Execute();
            }
        }

        public void OnNavigatedFrom(NavigationEventArgs e)
        {
        }
    }
}
