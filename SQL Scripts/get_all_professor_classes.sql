Delimiter //
create procedure get_all_professor_classes(
in professor_lastname varchar(40))
begin
select class_num, dept_id,section_number,room_id, professor_last_name
from schedule inner join professors on professor.professor_id=schedule.professor_id
where professor_last_name = professor.professor_last_name;
end//
DELIMITER ;