/*Table structure for table `requests` */
CREATE TABLE IF NOT EXISTS `requests` (
    `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
    `correlation_id` VARCHAR(100) NOT NULL,
    `executing_stub_id` VARCHAR(100) DEFAULT NULL,
    `request_begin_time` DATETIME NOT NULL,
    `request_end_time` DATETIME DEFAULT NULL,
    `json` LONGTEXT NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE KEY `ix_correlation_id` (`correlation_id`),
    KEY `ix_executing_stub_id` (`executing_stub_id`),
    KEY `ix_request_begin_time` (`request_begin_time`),
    KEY `ix_request_end_time` (`request_end_time`)
    ) ENGINE=INNODB AUTO_INCREMENT=1 DEFAULT CHARSET=UTF8MB4;

/*Table structure for table `stubs` */
CREATE TABLE IF NOT EXISTS `stubs` (
    `id` BIGINT(20) NOT NULL AUTO_INCREMENT,
    `stub_id` VARCHAR(255) NOT NULL,
    `stub` LONGTEXT NOT NULL,
    `stub_type` VARCHAR(20) NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE KEY `ix_stub_id` (`stub_id`)
    ) ENGINE=INNODB AUTO_INCREMENT=1 DEFAULT CHARSET=UTF8MB4;
