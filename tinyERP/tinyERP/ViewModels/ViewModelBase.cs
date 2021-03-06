﻿using System;
using System.Collections;
using System.ComponentModel;
using MvvmValidation;
using tinyERP.Dal;
using tinyERP.UI.Factories;

namespace tinyERP.UI.ViewModels
{
    public abstract class ViewModelBase : INotifyDataErrorInfo, INotifyPropertyChanged
    {
        private readonly IUnitOfWorkFactory factory;

        protected ViewModelBase(IUnitOfWorkFactory factory)
        {
            this.factory = factory;

            Validator = new ValidationHelper();
            NotifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(Validator);
            NotifyDataErrorInfoAdapter.ErrorsChanged += OnErrorChanged;
        }

        protected IUnitOfWork UnitOfWork { get; set; }

        protected ValidationHelper Validator { get; }

        private NotifyDataErrorInfoAdapter NotifyDataErrorInfoAdapter { get; }

        private void OnErrorChanged(object sender, DataErrorsChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        public void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public bool SetProperty<T>(ref T field, T value, string name, params string[] otherNames)
        {
            if (Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(name);
            foreach (var s in otherNames)
            {
                OnPropertyChanged(s);
            }
            return true;
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return NotifyDataErrorInfoAdapter.GetErrors(propertyName);
        }

        public bool HasErrors => NotifyDataErrorInfoAdapter.HasErrors;
   
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { NotifyDataErrorInfoAdapter.ErrorsChanged += value; }
            remove { NotifyDataErrorInfoAdapter.ErrorsChanged -= value; }
        }

        public void Init()
        {
            UnitOfWork = factory.GetUnitOfWork();
            Load();
        }

        public abstract void Load();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
