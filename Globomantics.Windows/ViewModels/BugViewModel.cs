using CommunityToolkit.Mvvm.Input;
using Globomantics.Domain;
using Globomantics.Infrastructure.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Globomantics.Windows.ViewModels;
public class BugViewModel : BaseTodoViewModel<Bug>
{
    private readonly IRepository<Bug> repository;

    private string? description;
    private string? affectedVersion;
    private int affectedUsers;
    private DateTimeOffset dueDate;
    private Severity severity;

 

    public string? Description
    {
        get => description;
        set
        {
            description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    public string? AffectedVersion
    {
        get => affectedVersion;
        set
        {
            affectedVersion = value;
            OnPropertyChanged(nameof(AffectedVersion));
        }
    }

    public int AffectedUsers
    {
        get => affectedUsers;
        set
        {
            affectedUsers = value;
            OnPropertyChanged(nameof(AffectedUsers));
        }
    }

    public DateTimeOffset DueDate
    {
        get => dueDate;
        set
        {
            dueDate = value;
            OnPropertyChanged(nameof(DueDate));
        }
    }

    public Severity Severity
    {
        get => severity;
        set
        {
            severity = value;
            OnPropertyChanged(nameof(Severity));
        }
    }

    public IEnumerable<Severity> SeverityLevels { get; } = new[]
    {
        Severity.Critical,
        Severity.Annoying,
        Severity.Major,
        Severity.Minor
    };

    public ObservableCollection<byte[]> Screenshots { get; set; } = new();

    public ICommand AttachScreenShotCommand { get; set; }



    public BugViewModel(IRepository<Bug> repository) : base()
    {
        this.repository = repository;

        SaveCommand = new RelayCommand(async () => await SaveAsync());

        AttachScreenShotCommand = new RelayCommand(() =>
        {
            var fileNames = ShowOpenFileDialog?.Invoke();
            if (fileNames is null || !fileNames.Any())
            {
                return;
            }
            foreach (var fileName in fileNames)
            {
                try
                {
                    var bytes = System.IO.File.ReadAllBytes(fileName);
                    Screenshots.Add(bytes);
                }
                catch (Exception ex)
                {
                    ShowError?.Invoke($"Failed to attach screenshot: {ex.Message}");
                }
            }
        });
    }

    public override void UpdateModel(Todo model)
    {
        if (model is not Bug bug) return;
        base.UpdateModel(bug);

        Description = bug.Description;
        AffectedVersion = bug.AffectedVersion;
        AffectedUsers = bug.AffectedUsers;
        Severity = bug.Severity;
        Screenshots = new(bug.Images);
        DueDate = bug.DueDate;
    }

    public override async Task SaveAsync()
    {
        if (string.IsNullOrEmpty(Title))
        {
            ShowError?.Invoke($"{nameof(Title)} cannot be empty.");
            return;
        }
        if (Model is null)
        {
            Model = new Bug(Title,
                Description ?? "No description",
                Severity,
                AffectedVersion ?? "",
                AffectedUsers,
                App.CurrentUser,
                App.CurrentUser,
                Screenshots.ToArray())
            {
                DueDate = DueDate,
                Parent = Parent,
                IsCompleted = IsCompleted
            };
        }
        else
        {
            Model = Model with
            {
                Title = Title,
                Description = Description ?? "No description",
                Severity = Severity,
                AffectedVersion = AffectedVersion ?? "",
                AffectedUsers = AffectedUsers,
                DueDate = DueDate,
                Parent = Parent,
                IsCompleted = IsCompleted,
                Images = Screenshots.ToArray(),
            };
        }
        await repository.AddAsync(Model);
        await repository.SaveChangesAsync();

        //TODO: Send message that item has been saved
    }
}

