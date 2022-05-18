USE classyschedule;
-- This script will create the time slot preference table,
-- this is a relationship between the professor and time slot tables
CREATE TABLE time_slot_preference (
    time_slot_id INT NOT NULL,
    prof_id INT NOT NULL,
    can_teach BOOLEAN,
    PRIMARY KEY (time_slot_id, prof_id),
    FOREIGN KEY (time_slot_id) REFERENCES time_slot(time_slot_id) ON DELETE CASCADE,
    FOREIGN KEY (prof_id) REFERENCES professor(professor_id) ON DELETE CASCADE
);
