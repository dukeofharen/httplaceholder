import os
import shutil

# TODO copy changelog

dir_path = os.path.dirname(os.path.realpath(__file__))
mkdocs_docs_path = os.path.join(dir_path, 'docs')

# Create mkdocs docs folder if it doesn't exist yet.
if not os.path.isdir(mkdocs_docs_path):
    os.mkdir(mkdocs_docs_path)

# Copy index.md to the correct location.
docs_md_path = os.path.join(dir_path, '..', 'docs.md')
docs_md_copy_path = os.path.join(mkdocs_docs_path, 'index.md')
shutil.copyfile(docs_md_path, docs_md_copy_path)

# Remove table of contents from copied file.
docs_md_file_read = open(docs_md_copy_path)
lines = docs_md_file_read.readlines()
docs_md_file_read.close()
lines_result = []
add_line = False
for line in lines:
    if line.startswith('# Installation'):
        add_line = True
    if add_line:
        lines_result.append(line)

docs_md_file_write = open(docs_md_copy_path, 'w')
docs_md_file_write.writelines(lines_result)
docs_md_file_write.close()

img_path = os.path.join(dir_path, '..', 'img')
img_copy_path = os.path.join(mkdocs_docs_path, 'img')
if os.path.isdir(img_copy_path):
    shutil.rmtree(img_copy_path)
shutil.copytree(img_path, img_copy_path)

samples_path = os.path.join(dir_path, '..', 'samples')
samples_copy_path = os.path.join(mkdocs_docs_path, 'samples')
if os.path.isdir(samples_copy_path):
    shutil.rmtree(samples_copy_path)
shutil.copytree(samples_path, samples_copy_path)
