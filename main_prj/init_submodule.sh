#check out,  git submodule
git submodule update --init --recursive
#auto change branche to  submodules
git submodule foreach -q --recursive 'branch="$(git config -f $toplevel/.gitmodules submodule.$name.branch)"; git checkout $branch'
