USE classyschedule;
-- This script will create the class_preference table
CREATE TABLE class_preference (
    class_id INT NOT NULL,
    prof_id INT NOT NULL,
    can_teach BOOLEAN,
    prefer_to_teach BOOLEAN,
    PRIMARY KEY (class_id, prof_id),
    FOREIGN KEY (class_id) REFERENCES class(class_id) ON DELETE CASCADE,
    FOREIGN KEY (prof_id) REFERENCES professor(professor_id) ON DELETE CASCADE
);
