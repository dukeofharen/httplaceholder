/*Add scenarios table*/
CREATE TABLE IF NOT EXISTS `scenarios`
(
    `id`               BIGINT(20)   NOT NULL AUTO_INCREMENT,
    `distribution_key` VARCHAR(300) NOT NULL DEFAULT '',
    `scenario`         VARCHAR(500) NOT NULL,
    `state`            VARCHAR(500) NOT NULL,
    `hit_count`        INT          NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE KEY `ix_scenario` (`scenario`),
    INDEX `ix_distribution_key` (`distribution_key`)
) ENGINE = INNODB
  AUTO_INCREMENT = 1
  DEFAULT CHARSET = UTF8MB4;
