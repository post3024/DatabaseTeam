USE classyschedule;
ALTER TABLE room
    ADD COLUMN building_name
    VARCHAR(4)
    NOT NULL;