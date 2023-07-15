--SAMPLE SKELETON OF FUNCTION CODE
create or replace function monkey_count ( _month integer,_day integer, _year integer,_days_interval integer)
returns table (dow varchar(10),counts bigint) as $$
declare
    start_date date := to_date(_year || '-' || _month || '-' || _day , 'YYYY-MM-DD');
    end_date date := start_date + _days_interval;
begin
    return query

end
$$ language plpgsql;
