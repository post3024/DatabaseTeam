USE classyschedule;
-- This script will create the section time slot table
CREATE TABLE section_time_slot(
    section_time_slot_id INTEGER AUTO_INCREMENT,
    start_time VARCHAR(6),
    end_time VARCHAR(6),
    on_monday BOOLEAN DEFAULT false,
    on_tuesday BOOLEAN DEFAULT false,
    on_wednesday BOOLEAN DEFAULT false,
    on_thursday BOOLEAN DEFAULT false,
    on_friday BOOLEAN DEFAULT false,
    PRIMARY KEY (section_time_slot_id)
);
