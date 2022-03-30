USE classyschedule;
CREATE TABLE department (
    dept_id INT NOT NULL AUTO_INCREMENT,
    dept_name VARCHAR(4) NOT NULL UNIQUE,
    PRIMARY KEY (dept_id)
);

INSERT INTO department (dept_name) VALUES ('CISC'), ('STAT');