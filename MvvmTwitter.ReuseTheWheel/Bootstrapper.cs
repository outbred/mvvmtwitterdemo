// Copyright (c) 2014 Brent Bulla Jr.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#region

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Windows;
using Infrastructure.Base;
using Infrastructure.Services.Interfaces;
using MEFedMVVM.ViewModelLocator;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using MvvmTwitter.ReuseTheWheel.Views;

#endregion

namespace MvvmTwitter.ReuseTheWheel
{
  public class Bootstrapper : MefBootstrapper, IComposer, IContainerProvider
  {
    private CompositionContainer _compositionContainer;

    protected override void ConfigureAggregateCatalog()
    {
      try
      {
        base.ConfigureAggregateCatalog();

        // infrastructure
        AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof (ViewModelBase).Assembly));
        // this assembly
        AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof (Bootstrapper).Assembly));

        // mefedmvvm services
        AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof (ViewModelLocator).Assembly));
      }
      catch (Exception ex)
      {
        Infrastructure.Helpers.Logger.Default.Error("Unable to configure catalog", ex);
        throw;
      }
    }

    protected override void InitializeShell()
    {
      base.InitializeShell();

      // VM base common services
      ViewModelBase.DispatcherService = _compositionContainer.GetExportedValue<IDispatcherService>();
      ViewModelBase.Aggregator = _compositionContainer.GetExportedValue<IEventAggregator>();
      ViewModelBase.MessageBoxService = _compositionContainer.GetExportedValue<IMessageBoxService>();
      ViewModelBase.FolderBrowserService = _compositionContainer.GetExportedValue<IFolderBrowserService>();
      ViewModelBase.RegionManager = _compositionContainer.GetExportedValue<IRegionManager>();
      ViewModelBase.Container = _compositionContainer.GetExportedValue<IServiceLocator>();

      Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                                            {
                                                              var shell = Shell as ShellView;
                                                              Application.Current.MainWindow = shell;

                                                              Application.Current.MainWindow.Show();
                                                              Application.Current.MainWindow.Activate();
                                                            }));
    }

    #region Overrides of Bootstrapper

    private object _shellViewModel;
    private ShellView _shellView;

    protected override DependencyObject CreateShell()
    {
      //init MEFedMVVM composed
      LocatorBootstrapper.ApplyComposer(this);

      _shellView = Container.GetExportedValue<ShellView>();

      return _shellView;
    }

    protected override CompositionContainer CreateContainer()
    {
      // Now add the MEF export provider for view models
      var exportProvider = new MEFedMVVMExportProvider(MEFedMVVMCatalog.CreateCatalog(AggregateCatalog));
      _compositionContainer = new CompositionContainer(null, true, exportProvider);
      exportProvider.SourceProvider = _compositionContainer;

      return _compositionContainer;
    }

    protected override IModuleCatalog CreateModuleCatalog()
    {
      return Microsoft.Practices.Prism.Modularity.ModuleCatalog.CreateFromXaml(new Uri("/MvvmTwitter.ReuseTheWheel;component/ModuleCatalog.xaml", UriKind.Relative));
    }

    #endregion

    #region Implementation of IComposer (For MEFedMVVM)

    public ComposablePartCatalog InitializeContainer()
    {
      //return the same catalog as the PRISM one
      return AggregateCatalog;
    }

    public IEnumerable<ExportProvider> GetCustomExportProviders()
    {
      //In case you want some custom export providers
      return null;
    }

    #endregion

    CompositionContainer IContainerProvider.CreateContainer()
    {
      // The MEFedMVVM call to create a container
      return _compositionContainer;
    }
  }
}