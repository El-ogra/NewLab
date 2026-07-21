using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using NewLab.Services.Interfaces;

namespace NewLab.Services.Implementations
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Stack<object> _history = new();
        private object? _currentViewModel;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? CurrentViewModel => _currentViewModel;

        public bool CanGoBack => _history.Count > 0;

        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            if (_currentViewModel != null)
            {
                _history.Push(_currentViewModel);
            }

            _currentViewModel = _serviceProvider.GetRequiredService<TViewModel>();
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
