USE classyschedule;
CREATE TABLE class (
    class_id INT NOT NULL AUTO_INCREMENT,
    class_num SMALLINT NOT NULL,
    dept_id INT NOT NULL,
    class_name VARCHAR(150) NULL,
    capacity SMALLINT NULL,
    credits TINYINT NULL,
    is_lab BOOLEAN NOT NULL,
    num_sections TINYINT NOT NULL,
    PRIMARY KEY (class_id),
    FOREIGN KEY (dept_id) REFERENCES department(dept_id) ON DELETE CASCADE
);