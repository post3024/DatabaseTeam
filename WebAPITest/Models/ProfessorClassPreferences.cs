using System;
using System.Collections.Generic;

// this model handles the schema for a professor and their preferences
namespace WebAPITest.Models
{
    public class ProfessorClassPreferencesDTO
    {
        public int prof_id { get; set; }

        public List<ClassPreferenceDTO> class_preferences { get; set; }

        // constructor
        public ProfessorClassPreferencesDTO(int prof_id, List<ClassPreferenceDTO> class_preferences)
        {
            this.prof_id = prof_id;
            this.class_preferences = class_preferences;
        }
        public ProfessorClassPreferencesDTO()
        {
        }
    }
}
