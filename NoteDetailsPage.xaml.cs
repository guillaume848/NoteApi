using Notes.Models;
using Notes.viewModels;
using viewModels.Notes;
using System;

namespace Notes
{
    public partial class NoteDetailsPage : ContentPage
    {
        public NoteDetailsPage(NotesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }

}