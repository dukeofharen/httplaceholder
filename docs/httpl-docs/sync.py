import os
import shutil

dir_path = os.path.dirname(os.path.realpath(__file__))
mkdocs_docs_path = os.path.join(dir_path, 'docs')

if not os.path.isdir(mkdocs_docs_path):
    os.mkdir(mkdocs_docs_path)

docs_md_path = os.path.join(dir_path, '..', 'docs.md')
docs_md_copy_path = os.path.join(mkdocs_docs_path, 'index.md')
shutil.copyfile(docs_md_path, docs_md_copy_path)

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
