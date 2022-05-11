USE classyschedule;
CREATE TABLE day_of_week_preference (
	prof_id INT NOT NULL,
    prefer_monday BOOLEAN,
    prefer_tuesday BOOLEAN,
    prefer_wednesday BOOLEAN,
    prefer_thursday BOOLEAN,
    prefer_friday BOOLEAN,
    PRIMARY KEY (prof_id),
    FOREIGN KEY (prof_id) REFERENCES professor(professor_id) ON DELETE CASCADE
);