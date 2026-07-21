using System;
using System.Collections.Generic;
using NewLab.Services.Interfaces;
using NewLab.ViewModels.Base;

namespace NewLab.Services.Implementations
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<ViewModelBase> _history = new();
        private ViewModelBase? _currentViewModel;

        public ViewModelBase? CurrentViewModel => _currentViewModel;

        public bool CanGoBack => _history.Count > 0;

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            if (_currentViewModel != null)
            {
                _history.Push(_currentViewModel);
            }

            _currentViewModel = Activator.CreateInstance<TViewModel>();
        }

        public void GoBack()
        {
            if (_history.Count > 0)
            {
                _currentViewModel = _history.Pop();
            }
        }
    }
}
