﻿using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using View = Epi.View;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewEntityListViewModel : DocumentViewModel
    {
        public class ViewEntityListChildViewModel : ListViewModel<ResponderEntity>
        {
            public ViewEntityRepository<ViewEntity> Repository { get; private set; }

            public ViewEntityListChildViewModel(View view)
            {
                Repository = new ViewEntityRepository<ViewEntity>(Context.Database, view);
                Refresh();
            }

            protected override IEnumerable<ResponderEntity> GetItems()
            {
                return Repository.SelectOrdered().WithResponders(Context);
            }
        }

        public View View { get; private set; }
        public ICollection<DataGridColumn> Columns { get; private set; }
        public ViewEntityListChildViewModel Entities { get; private set; }

        public ICommand CreateCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand PostpopulateCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand UndeleteCommand { get; private set; }

        public ViewEntityListViewModel(View view)
        {
            Title = view.Name;
            View = view;
            Columns = DataGridExtensions.GetDataColumns(Context.Project, view, view.Id != Context.Responders.View.Id).ToList();
            Entities = new ViewEntityListChildViewModel(view);
            CreateCommand = new AsyncCommand(CreateAsync);
            EditCommand = new AsyncCommand(EditAsync, Entities.HasOneSelectedItem);
            PostpopulateCommand = new AsyncCommand(PostpopulateAsync, CanPostpopulate);
            DeleteCommand = new Command(Delete, Entities.HasAnySelectedItems);
            UndeleteCommand = new Command(Undelete, Entities.HasAnySelectedItems);
        }

        public async Task CreateAsync()
        {
            Domain.View view = Context.Views.SelectById(View.Id);
            if (view.HasResponderIdField)
            {
                PrepopulateViewModel model = new PrepopulateViewModel(view);
                await ServiceLocator.Dialog.ShowAsync(model);
            }
            else
            {
                Wrapper wrapper = Enter.OpenNewRecord.Create(Context.Project.FilePath, View.Name);
                wrapper.AddRecordSavedHandler();
                await ServiceLocator.Wrapper.InvokeAsync(wrapper);
            }
        }

        public async Task EditAsync()
        {
            Wrapper wrapper = Enter.OpenRecord.Create(Context.Project.FilePath, View.Name, Entities.SelectedItems.First().UniqueKey.Value);
            wrapper.AddRecordSavedHandler();
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }

        public bool CanPostpopulate()
        {
            return View.Fields.Contains(FieldNames.ResponderId) && Entities.HasOneSelectedItem();
        }

        public async Task PostpopulateAsync()
        {
            ViewEntity entity = Entities.Repository.Refresh(Entities.SelectedItems.First());
            PostpopulateViewModel model = new PostpopulateViewModel(View, entity);
            model.Saved += (sender, e) =>
            {
                Entities.Refresh();
            };
            await ServiceLocator.Dialog.ShowAsync(model);
        }

        private void SetDeleted(bool deleted)
        {
            using (ServiceLocator.Busy.Begin())
            {
                foreach (ViewEntity entity in Entities.Repository.Refresh(Entities.SelectedItems))
                {
                    if (entity.Deleted != deleted)
                    {
                        entity.Deleted = deleted;
                        Entities.Repository.Save(entity);
                    }
                }
            }
            Entities.Refresh();
        }

        public void Delete()
        {
            SetDeleted(true);
        }

        public void Undelete()
        {
            SetDeleted(false);
        }
    }
}