/*Table structure for table `metadata` */
CREATE TABLE IF NOT EXISTS `metadata` (
    `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
    `stub_update_tracking_id` VARCHAR(50) NOT NULL,
    PRIMARY KEY(`id`)
    ) ENGINE=INNODB AUTO_INCREMENT=1 DEFAULT CHARSET=UTF8MB4;

/*Insert initial data in the metadata table*/
INSERT INTO `metadata` (stub_update_tracking_id)
SELECT 'initial_value'
FROM dual
WHERE NOT EXISTS (SELECT * FROM metadata);
