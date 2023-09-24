/*Add scenarios table*/
CREATE TABLE IF NOT EXISTS `scenarios`
(
    `id`        BIGINT(20)   NOT NULL AUTO_INCREMENT,
    `scenario`  VARCHAR(500) NOT NULL,
    `state`     VARCHAR(500) NOT NULL,
    `hit_count` INT          NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE KEY `ix_scenario` (`scenario`)
) ENGINE = INNODB
  AUTO_INCREMENT = 1
  DEFAULT CHARSET = UTF8MB4;
