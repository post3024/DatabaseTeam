CREATE TABLE section_time_slot(
    section_time_slot_id INTEGER AUTO_INCREMENT,
    on_monday BOOLEAN DEFAULT false,
    on_tuesday BOOLEAN DEFAULT false,
    on_wednesday BOOLEAN DEFAULT false,
    on_thursday BOOLEAN DEFAULT false,
    on_friday BOOLEAN DEFAULT false,
    time_slot_id INTEGER NOT NULL,
    PRIMARY KEY (section_time_slot_id),
    FOREIGN KEY (time_slot_id) REFERENCES time_slot(time_slot_id) ON DELETE CASCADE,
    UNIQUE (on_monday, on_tuesday, on_wednesday, on_thursday, on_friday, time_slot_id)
);
