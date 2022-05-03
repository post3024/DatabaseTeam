USE classyschedule;
CREATE TABLE class (
    class_num SMALLINT NOT NULL,
    dept_id INT NOT NULL,
    class_name VARCHAR(150) NULL,
    capacity SMALLINT NULL,
    credits TINYINT NULL,
    is_lab BOOLEAN NOT NULL,
    FOREIGN KEY (dept_id)
		REFERENCES department(dept_id)
        ON DELETE CASCADE,
	PRIMARY KEY (class_num, dept_id, is_lab)
);