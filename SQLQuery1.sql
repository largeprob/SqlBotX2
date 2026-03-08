with  ct1 as (select 2025 as year,130 as c1),
ct2 as (select 2024 as year,100 as c2),
ct3 as (select 2026 as year,1001 as r2)
select  FORMAT(((ct1.c1 - ct2.c2) * 1.0 / ct2.c2 * 100), 'N2') + '%' AS '2025年新增客户同比',ct3.r2 as '2026年新增客户总数'
from ct1,ct2,ct3
 
CREATE TABLE #sales (
    sale_date DATE,          -- 销售日期
    region VARCHAR(50),      -- 地区
    dept VARCHAR(50),        -- 创建部门
    sales_amount DECIMAL(10,2), -- 销售额
    profit DECIMAL(10,2),    -- 利润
    expense DECIMAL(10,2)    -- 支出
);
INSERT INTO #sales (sale_date, region, dept, sales_amount, profit, expense)
VALUES
-- 2025 年数据（用于计算同比）
('2025-01-15', 'North', 'Sales', 1000.00, 200.00, 50.00),
('2025-01-20', 'North', 'Sales', 1500.00, 300.00, 70.00),   -- 同月合计：销售额 2500
('2025-01-10', 'North', 'Marketing', 800.00, 160.00, 40.00),
('2025-01-12', 'South', 'Sales', 1200.00, 240.00, 60.00),
('2025-02-05', 'North', 'Sales', 1100.00, 220.00, 55.00),
('2025-02-18', 'South', 'Sales', 1300.00, 260.00, 65.00),
('2025-03-12', 'North', 'Sales', 1400.00, 280.00, 70.00),
('2025-03-22', 'South', 'Marketing', 900.00, 180.00, 45.00),
('2026-01-11', 'North', 'Sales', 3000.00, 600.00, 80.00),
('2026-01-19', 'North', 'Sales', 2800.00, 560.00, 90.00),   -- 同月合计：销售额 5800（同比 (5800-2500)/2500 = 132%）
('2026-01-08', 'North', 'Marketing', 1600.00, 320.00, 50.00), -- 销售额 1600（同比 (1600-800)/800 = 100%）
('2026-01-14', 'South', 'Sales', 2000.00, 400.00, 70.00),     -- 销售额 2000（同比 (2000-1200)/1200 = 66.7%）
('2026-02-03', 'North', 'Sales', 1300.00, 260.00, 60.00),     -- 同比 (1300-1100)/1100 = 18.2%
('2026-02-21', 'South', 'Sales', 1500.00, 300.00, 68.00),     -- 同比 (1500-1300)/1300 = 15.4%
('2026-03-07', 'North', 'Sales', 1450.00, 290.00, 72.00),     -- 同比 (1450-1400)/1400 = 3.6% (<10%)
('2026-03-25', 'South', 'Marketing', 1100.00, 220.00, 48.00);  -- 同比 (1100-900)/900 = 22.2%

select  * from #sales;

with  ct1 as (
    select  MONTH(sale_date) as 月, region as 地区,  dept as 部门,
    SUM(expense) as r1,SUM(sales_amount) as c1,SUM(profit) as c3
    from #sales
    WHERE sale_date   >= '2026-01-01' and  sale_date <  '2027-01-01'
    group by MONTH(sale_date),region,dept
),
ct2 as (
    select  MONTH(sale_date) as 月, region as 地区,  dept as 部门,
    SUM(sales_amount) as c2,SUM(profit) as c4
    from #sales
    WHERE sale_date   >= '2025-01-01' and  sale_date <  '2026-01-01'
    group by MONTH(sale_date),region,dept
)
select  
 ct1.月, ct1.地区,ct1.部门,
(ct1.c1-ct2.c2)/ct2.c2  * 100 as '2025年每月的销售额同比增长率',
(ct1.c3-ct2.c4)/ct2.c4 * 100 as '2026年每月的利润同比增长率',
r1 as '2026年每月支出总额'
from ct1
LEFT JOIN ct2 on ct1.月 = ct2.月 and ct1.地区 = ct2.地区 and ct1.部门 = ct2.部门 
 
 