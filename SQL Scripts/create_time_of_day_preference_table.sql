USE classyschedule;
CREATE TABLE time_of_day_preference (
    prof_id INT NOT NULL,
    prefer_morning BOOLEAN,
    prefer_afternoon BOOLEAN,
    prefer_evening BOOLEAN,
    PRIMARY KEY (prof_id),
    FOREIGN KEY (prof_id) REFERENCES professor(professor_id)
);