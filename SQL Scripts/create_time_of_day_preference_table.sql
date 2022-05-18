USE classyschedule;
-- This script will create the time of day preference table for the professor
CREATE TABLE time_of_day_preference (
    prof_id INT NOT NULL,
    prefer_morning BOOLEAN,
    prefer_afternoon BOOLEAN,
    prefer_evening BOOLEAN,
    PRIMARY KEY (prof_id),
    FOREIGN KEY (prof_id) REFERENCES professor(professor_id) ON DELETE CASCADE
);
