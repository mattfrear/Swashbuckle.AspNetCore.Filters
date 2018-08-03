# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]
Nothing here yet

## [4.0.3] - 2018-08-03
### Fixed
- Issue #54 where child objects weren't having their descriptions set if parent property was missing `[Description]`

## [4.0.2] - 2018-07-26
### Fixed
- Issue #54 where arrays of child objects weren't having their description set

## [4.0.1] - 2018-07-24
### Fixed
- Fix bug where generic types weren't being automatically annotated

## [4.0.0] - 2018-07-23
### Added
- Automatic annotation of request and response examples
### Changed
- How ExamplesOperationFilter is installed - see Installation section of the Readme

## [3.1.0] - 2018-07-21
### Added
- SecurityRequirementsOperationFilter to correctly set bearer token stuff when using `[Authorize]`
