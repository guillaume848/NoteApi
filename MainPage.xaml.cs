using viewModels.Notes;
using Notes.Models;
using System;

namespace Notes
{
    public partial class MainPage : ContentPage
    {
        private NotesViewModel _viewModel;

        public MainPage(NotesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        private async void OnAddNoteClicked(object sender, EventArgs e)
        {
            string title = await DisplayPromptAsync("New Note", "Enter note title:");
            string content = await DisplayPromptAsync("New Note", "Enter note content:");

            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
            {
                var newNote = new Note
                {
                    Title = title,
                    Content = content,
                    DateCreated = DateTime.Now
                };

                await _viewModel.AddNoteAsync(newNote);
            }
        }

        private async void OnEditNoteClicked(object sender, EventArgs e)
        {
            if (_viewModel.SelectedNote == null) return;

            string title = await DisplayPromptAsync("Edit Note", "Edit note title:", initialValue: _viewModel.SelectedNote.Title);
            string content = await DisplayPromptAsync("Edit Note", "Edit note content:", initialValue: _viewModel.SelectedNote.Content);

            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
            {
                _viewModel.SelectedNote.Title = title;
                _viewModel.SelectedNote.Content = content;
                await _viewModel.UpdateNoteAsync(_viewModel.SelectedNote);
            }
        }

        private async void OnDeleteNoteClicked(object sender, EventArgs e)
        {
            if (_viewModel.SelectedNote == null) return;

            bool confirm = await DisplayAlert("Delete Note", "Are you sure you want to delete this note?", "Yes", "No");
            if (confirm)
            {
                await _viewModel.DeleteNoteAsync();
            }
        }
    }
}
