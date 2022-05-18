USE classyschedule;
--This script will create teh department table
CREATE TABLE department (
    dept_id INT NOT NULL AUTO_INCREMENT,
    dept_name VARCHAR(4) NOT NULL UNIQUE,
    PRIMARY KEY (dept_id)
);
-- Insert the STAT and CISC records into the table
INSERT INTO department (dept_name) 
VALUES ('CISC'), ('STAT');
