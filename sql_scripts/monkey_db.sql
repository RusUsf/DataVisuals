--Check for the initial data records
select * from monkey_data;

--Granting privileges to user burner
grant all privileges on schema public to burner;
grant all privileges on all tables in schema public to burner;

--Randomly inserting records into 'monkey_data' table
insert into monkey_data (date,event)
select now() - interval '10 day' * floor(RANDOM()* 7), event
from monkey_data;

--Check for the inserted values
select * from monkey_data limit 15;
select count(*) from monkey_data;

--Creating a function to be called from C# Console Application
select extract(dow from date) as doworder
,case
when extract(dow from date) = 0 then 'Sunday'
when extract(dow from date) = 1 then 'Monday'
when extract(dow from date) = 2 then 'Tuesday'
when extract(dow from date) = 3 then 'Wednesday'
when extract(dow from date) = 4 then 'Thursday'
when extract(dow from date) = 5 then 'Friday'
when extract(dow from date) = 6 then 'Saturday'
end as dow
,count(*) - 16000 as counts
from monkey_data
group by 1,2
order by 1 asc;


--SAMPLE SKELETON FUNCTION CODE
create or replace function monkey_count ( _month integer,_day integer, _year integer,_days_interval integer)
returns table (dow varchar(10),counts bigint) as $$
declare
    start_date date := to_date(_year || '-' || _month || '-' || _day , 'YYYY-MM-DD');
    end_date date := start_date + _days_interval;
begin
    return query

end
$$ language plpgsql;

--SQL to be Wrapped into the Function
with cte_my_counts (doworder,dow,counts) as
(
    select extract(dow from date) as doworder
,cast(case
when extract(dow from date) = 0 then 'Sunday'
when extract(dow from date) = 1 then 'Monday'
when extract(dow from date) = 2 then 'Tuesday'
when extract(dow from date) = 3 then 'Wednesday'
when extract(dow from date) = 4 then 'Thursday'
when extract(dow from date) = 5 then 'Friday'
when extract(dow from date) = 6 then 'Saturday'
end as varchar(10)) as dow
,count(*) - 16000 as counts
from monkey_data
group by 1,2
order by 1 asc
)
select dow,counts from cte_my_counts

--Creating 'monkey_count' function
create or replace function monkey_count ( _month integer,_day integer, _year integer,_days_interval integer)
returns table (dow varchar(10),counts bigint) as $$
declare
    start_date date := to_date(_year || '-' || _month || '-' || _day , 'YYYY-MM-DD');
    end_date date := start_date + _days_interval;
begin
    return query
with cte_my_counts (doworder,dow,counts) as
(
    select extract(dow from date) as doworder
,cast(case
when extract(dow from date) = 0 then 'Sunday'
when extract(dow from date) = 1 then 'Monday'
when extract(dow from date) = 2 then 'Tuesday'
when extract(dow from date) = 3 then 'Wednesday'
when extract(dow from date) = 4 then 'Thursday'
when extract(dow from date) = 5 then 'Friday'
when extract(dow from date) = 6 then 'Saturday'
end as varchar(10)) as dow
,(count(*))::bigint as counts
from monkey_data
where 
    date >= start_date
    and
    date < end_date
group by 1,2
order by 1 asc
)
select a.dow,a.counts 
from cte_my_counts a;
end;
$$ language plpgsql;


--Check
select * from monkey_count(1,1,2023,300);

select oid from pg_proc

--Function Definition
select pg_get_functiondef(oid),1
from pg_proc
where proname = 'monkey_count';

