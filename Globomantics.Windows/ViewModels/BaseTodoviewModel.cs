using CommunityToolkit.Mvvm.ComponentModel;
using Globomantics.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using CommunityToolkit.Mvvm.Input;

namespace Globomantics.Windows.ViewModels;

public abstract class BaseTodoviewModel<T> : ObservableObject, ITodoViewModel
    where T : Todo
{
    private T? model;
    private string? title;
    private bool isCompleted;
    private Todo? parent;

    public Todo? Parent
    {
        get => parent;
        set
        {
            parent = value;
            OnPropertyChanged(nameof(Parent));
        }
    }

    public bool IsCompleted
    {
        get => isCompleted;
        set
        {
            isCompleted = value;
            OnPropertyChanged(nameof(IsCompleted));
        }
    }

    public string? Title
    {
        get => title;
        set
        {
            title = value;
            OnPropertyChanged(nameof(Title));
        }
    }

    public T? Model
    {
        get => model;
        set
        {
            model = value;
            OnPropertyChanged(nameof(Model));
            OnPropertyChanged(nameof(IsExisting));
        }
    }

    public bool IsExisting => Model is not null;


    #region From ITodoViewModel to IViewModel
    public IEnumerable<Todo>? AvailableParentTasks { get; set; }

    public ICommand DeleteCommand { get; }

    public ICommand? SaveCommand { get; set; } = default!;

    public Action<string>? ShowAlert { get; set; }
    public Action<string>? ShowError { get; set; }
    public Func<IEnumerable<string>>? ShowOpenFileDialog { get; set; }
    public Func<string>? ShowSaveFileDialog { get; set; }
    public Func<string, bool>? AskForConfirmation { get; set; }
    #endregion

    public abstract Task SaveAsync();

    public virtual void UpdateModel(Todo model)
    {
        if(model is null)
        {
            return;
        }

        var parent = AvailableParentTasks?.SingleOrDefault(
            t => t.Parent is not null && t.Parent?.Id == model.Parent?.Id
        );

        Model = model as T;
        Title = model.Title;
        IsCompleted = model.IsCompleted;
        Parent = parent;
    }

    public BaseTodoviewModel() 
    {
        DeleteCommand = new RelayCommand(() =>
        {
            if (Model is not null)
            {
                Model = Model with { IsDeleted = true };
            }
        });
    }
}
