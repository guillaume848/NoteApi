using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Notes.Models;
using System.Windows.Input;
using Notes.viewModels;

namespace viewModels.Notes
{
    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public enum SortField
    {
        Title,
        DateCreated
    }

    public class NotesViewModel : INotifyPropertyChanged
    {
        private readonly NotesApiService _apiService;

        private ObservableCollection<Note> notes;
        public ObservableCollection<Note> Notes
        {
            get => notes;
            set
            {
                notes = value;
                OnPropertyChanged();
            }
        }

        private Note selectedNote;
        public Note SelectedNote
        {
            get => selectedNote;
            set
            {
                selectedNote = value;
                OnPropertyChanged();
            }
        }

        public ICommand SortByTitleAscCommand { get; }
        public ICommand SortByTitleDescCommand { get; }
        public ICommand SortByDateAscCommand { get; }
        public ICommand SortByDateDescCommand { get; }

        private ObservableCollection<Note> allNotes = new ObservableCollection<Note>(); // Store the unfiltered notes
        private string searchQuery;
        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                OnPropertyChanged();
                FilterNotes(); // Trigger filtering when the query changes
            }
        }

        private bool isNoResultsVisible;
        public bool IsNoResultsVisible
        {
            get => isNoResultsVisible;
            set
            {
                isNoResultsVisible = value;
                OnPropertyChanged();
            }
        }

        private SortOrder sortOrder = SortOrder.Ascending;
        public SortOrder SortOrder
        {
            get => sortOrder;
            set
            {
                sortOrder = value;
                OnPropertyChanged();
                FilterNotes(); // Reapply filtering and sorting when the sort order changes
            }
        }

        private SortField sortField = SortField.DateCreated;
        public SortField SortField
        {
            get => sortField;
            set
            {
                sortField = value;
                OnPropertyChanged();
                FilterNotes(); // Reapply sorting when the field changes
            }
        }

        public NotesViewModel(NotesApiService apiService)
        {
            _apiService = apiService;

            SortByTitleAscCommand = new Command(() =>
            {
                SortField = SortField.Title;
                SortOrder = SortOrder.Ascending;
            });

            SortByTitleDescCommand = new Command(() =>
            {
                SortField = SortField.Title;
                SortOrder = SortOrder.Descending;
            });

            SortByDateAscCommand = new Command(() =>
            {
                SortField = SortField.DateCreated;
                SortOrder = SortOrder.Ascending;
            });

            SortByDateDescCommand = new Command(() =>
            {
                SortField = SortField.DateCreated;
                SortOrder = SortOrder.Descending;
            });

            // Load notes initially
            LoadNotesAsync();
        }

        private async void LoadNotesAsync()
        {
            try
            {
                var notesList = await _apiService.GetNotesAsync();
                allNotes = new ObservableCollection<Note>(notesList);
                Notes = new ObservableCollection<Note>(notesList);

                // Update the visibility of the "No results found" label
                IsNoResultsVisible = Notes.Count == 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading notes: {ex.Message}");
            }
        }

        public async Task AddNoteAsync(Note note)
        {
            try
            {
                var response = await _apiService.AddNoteAsync(note);
                if (response != null)
                {
                    allNotes.Add(response);
                    FilterNotes();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding note: {ex.Message}");
            }
        }

        public async Task UpdateNoteAsync(Note note)
        {
            try
            {
                await _apiService.UpdateNoteAsync(note);
                var existingNote = allNotes.FirstOrDefault(n => n.Id == note.Id);
                if (existingNote != null)
                {
                    var index = allNotes.IndexOf(existingNote);
                    allNotes[index] = note;
                }
                FilterNotes();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating note: {ex.Message}");
            }
        }

        public async Task DeleteNoteAsync()
        {
            try
            {
                if (SelectedNote != null)
                {
                    await _apiService.DeleteNoteAsync(SelectedNote.Id);
                    allNotes.Remove(SelectedNote);
                    FilterNotes();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting note: {ex.Message}");
            }
        }

        private void FilterNotes()
        {
            IEnumerable<Note> filteredNotes;

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredNotes = allNotes;
            }
            else
            {
                filteredNotes = allNotes.Where(note =>
                    note.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    note.Content.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
            }

            filteredNotes = SortField switch
            {
                SortField.Title => SortOrder == SortOrder.Ascending
                    ? filteredNotes.OrderBy(note => note.Title)
                    : filteredNotes.OrderByDescending(note => note.Title),
                SortField.DateCreated => SortOrder == SortOrder.Ascending
                    ? filteredNotes.OrderBy(note => note.DateCreated)
                    : filteredNotes.OrderByDescending(note => note.DateCreated),
                _ => filteredNotes
            };

            Notes = new ObservableCollection<Note>(filteredNotes);
            IsNoResultsVisible = Notes.Count == 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
