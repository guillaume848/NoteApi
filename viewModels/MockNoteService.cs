using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using viewModels.Notes;
using Notes.Models;
using System.Net.Http;
using System.Net.Http.Json;

namespace Notes.viewModels
{
    public class NotesApiService
    {
        private readonly HttpClient _httpClient;

        public NotesApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Note>> GetNotesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Note>>("api/notes");
        }

        public async Task<Note> AddNoteAsync(Note note)
        {
            var response = await _httpClient.PostAsJsonAsync("api/notes", note);

            if (response.IsSuccessStatusCode)
            {
                // Deserialize and return the newly created note
                return await response.Content.ReadFromJsonAsync<Note>();
            }

            throw new Exception($"Error adding note: {response.ReasonPhrase}");
        }

        public async Task UpdateNoteAsync(Note note)
        {
            await _httpClient.PutAsJsonAsync($"api/notes/{note.Id}", note);
        }

        public async Task DeleteNoteAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/notes/{id}");
        }
    }
}
